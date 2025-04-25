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

public class MissionStatusUpdater : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MissionStatusUpdater> _logger;
    private readonly TimeZoneInfo _localTimeZone;

    public MissionStatusUpdater(IServiceProvider serviceProvider, ILogger<MissionStatusUpdater> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _localTimeZone = TimeZoneInfo.CreateCustomTimeZone("UTC+1", TimeSpan.FromHours(1), "UTC+1", "UTC+1");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MissionStatusUpdater is running.");

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

                    // 1. Create notification 30 min before missions
                    var upcomingMissions = await _context.Missions
                        .Where(m => m.StartTime <= thresholdTime && m.StartTime > nowLocal)
                        .ToListAsync(stoppingToken);

                    foreach (var mission in upcomingMissions)
                    {
                        bool alreadyNotified = await _context.Notifications
                            .AnyAsync(n => n.Type == "mission" &&
                                           n.Details!.Contains($"Mission ID: {mission.Id}"),
                                           stoppingToken);

                        if (!alreadyNotified)
                        {
                            var notification = new Notification
                            {
                                Type = "mission",
                                Details = $"Mission ID: {mission.Id} scheduled at {mission.StartTime} is due in 30 minutes.",
                                RecipientId = mission.AdminId,
                                ReferenceId = mission.Id
                            };

                            _context.Notifications.Add(notification);
                        }
                    }

                    // 2. Start missions
                    var missionsToStart = await _context.Missions
                        .Where(m => m.StartTime <= nowLocal && m.Status != "Started" && m.Status != "Accomplished")
                        .ToListAsync();

                    foreach (var mission in missionsToStart)
                    {
                        mission.Status = "Started";
                        _logger.LogInformation($"Mission {mission.Id} started at {nowLocal}.");
                    }

                    // 3. Accomplish missions
                    var missionsToFinish = await _context.Missions
                        .Where(m => m.EndTime <= nowLocal && m.Status == "Started")
                        .ToListAsync();

                    foreach (var mission in missionsToFinish)
                    {
                        mission.Status = "Accomplished";
                        _logger.LogInformation($"Mission {mission.Id} accomplished at {nowLocal}.");
                    }

                    if (missionsToStart.Any() || missionsToFinish.Any() || upcomingMissions.Any())
                    {
                        await _context.SaveChangesAsync(stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mission statuses.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
