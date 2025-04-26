using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WeatherApi;
using sp_backend_March4.Models;

public class TrainingStatusUpdater : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TrainingStatusUpdater> _logger;
    private readonly TimeZoneInfo _localTimeZone;

    public TrainingStatusUpdater(IServiceProvider serviceProvider, ILogger<TrainingStatusUpdater> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _localTimeZone = TimeZoneInfo.CreateCustomTimeZone("UTC+1", TimeSpan.FromHours(1), "UTC+1", "UTC+1");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TrainingStatusUpdater is running.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var nowUtc = DateTime.UtcNow;
                    var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, _localTimeZone);
                    var thresholdTime = nowLocal.AddMinutes(30);

                    // 1. Notify about upcoming trainings (starting within next 30 minutes)
                    var upcomingTrainings = await _context.Trainings
                        .Where(t => t.StartTime > nowLocal && t.StartTime <= thresholdTime)
                        .ToListAsync(stoppingToken);

                    foreach (var training in upcomingTrainings)
                    {
                        bool alreadyNotified = await _context.Notifications
                            .AnyAsync(n => n.Type == "training" &&
                                           n.Details!.Contains($"Training ID: {training.Id}"),
                                           stoppingToken);

                        if (!alreadyNotified)
                        {
                            var notification = new Notification
                            {
                                Type = "Training",
                                Details = $"Training ID: {training.Id} scheduled at {training.StartTime} is due in 30 minutes.",
                                ReferenceId = training.Id

                            };

                            _context.Notifications.Add(notification);
                        }
                    }

                    // 2. Update status to "Ongoing"
                    var trainingsToStart = await _context.Trainings
                        .Where(t => t.StartTime <= nowLocal && t.Status != "Ongoing" && t.Status != "Completed")
                        .ToListAsync(stoppingToken);

                    foreach (var training in trainingsToStart)
                    {
                        training.Status = "Ongoing";
                        _logger.LogInformation($"Training {training.Id} started at {nowLocal}.");
                    }

                    // 3. Update status to "Completed"
                    var trainingsToFinish = await _context.Trainings
                        .Where(t => t.EndTime <= nowLocal && t.Status == "Ongoing")
                        .ToListAsync(stoppingToken);

                    foreach (var training in trainingsToFinish)
                    {
                        training.Status = "Completed";
                        _logger.LogInformation($"Training {training.Id} completed at {nowLocal}.");
                    }

                    if (trainingsToStart.Any() || trainingsToFinish.Any() || upcomingTrainings.Any())
                    {
                        await _context.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating training statuses.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
