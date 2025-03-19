using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WeatherApi;

public class TrainingStatusUpdater : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TrainingStatusUpdater> _logger;
    private readonly TimeZoneInfo _localTimeZone;

    public TrainingStatusUpdater(IServiceProvider serviceProvider, ILogger<TrainingStatusUpdater> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"); // UTC+1
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

                    // Convert UTC to local time (UTC+1)
                    var nowUtc = DateTime.UtcNow;
                    var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, _localTimeZone);

                    // Find trainings that need to be updated
                    var trainingsToStart = await _context.Trainings
                        .Where(t => t.StartTime <= nowLocal && t.Status != "Ongoing" && t.Status != "Completed")
                        .ToListAsync();

                    var trainingsToFinish = await _context.Trainings
                        .Where(t => t.EndTime <= nowLocal && t.Status == "Ongoing")
                        .ToListAsync();

                    // Update training statuses
                    foreach (var training in trainingsToStart)
                    {
                        training.Status = "Ongoing";
                        _logger.LogInformation($"Training {training.Id} started at {nowLocal}.");
                    }

                    foreach (var training in trainingsToFinish)
                    {
                        training.Status = "Completed";
                        _logger.LogInformation($"Training {training.Id} completed at {nowLocal}.");
                    }

                    if (trainingsToStart.Any() || trainingsToFinish.Any())
                    {
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating training statuses.");
            }

            // Wait for a minute before checking again
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
