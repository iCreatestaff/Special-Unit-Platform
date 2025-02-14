using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WeatherApi;

public class MissionStatusUpdater : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MissionStatusUpdater> _logger;

    public MissionStatusUpdater(IServiceProvider serviceProvider, ILogger<MissionStatusUpdater> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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

                    var now = DateTime.UtcNow; // Use UTC to avoid timezone issues

                    // Find missions that need to be updated
                    var missionsToStart = await _context.Missions
                        .Where(m => m.StartTime <= now && m.Status != "Started" && m.Status != "Accomplished")
                        .ToListAsync();

                    var missionsToFinish = await _context.Missions
                        .Where(m => m.EndTime <= now && m.Status == "Started")
                        .ToListAsync();

                    // Update mission statuses
                    foreach (var mission in missionsToStart)
                    {
                        mission.Status = "Started";
                        _logger.LogInformation($"Mission {mission.Id} started.");
                    }

                    foreach (var mission in missionsToFinish)
                    {
                        mission.Status = "Accomplished";
                        _logger.LogInformation($"Mission {mission.Id} accomplished.");
                    }

                    if (missionsToStart.Any() || missionsToFinish.Any())
                    {
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating mission statuses.");
            }

            // Wait for a minute before checking again
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
