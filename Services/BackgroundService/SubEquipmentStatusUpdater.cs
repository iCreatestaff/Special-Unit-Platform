using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeatherApi;

namespace sp_backend.Services
{
    public class SubEquipmentStatusUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<SubEquipmentStatusUpdater> _logger;
        private readonly TimeZoneInfo _localTimeZone;

        public SubEquipmentStatusUpdater(IServiceScopeFactory serviceScopeFactory, ILogger<SubEquipmentStatusUpdater> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _localTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SubEquipmentStatusUpdater Service is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _localTimeZone);

                        // Find all sub-equipment affected by nonavailability periods
                        var allSubEquipmentIds = await dbContext.SubEquipments
                            .Select(se => se.Id)
                            .ToListAsync(stoppingToken);

                        foreach (var subEquipmentId in allSubEquipmentIds)
                        {
                            // Check if there is any active nonavailability period for this sub-equipment
                            bool isInMaintenance = await dbContext.Nonavailabilities
                                .AnyAsync(n => n.SubEquipmentId == subEquipmentId && n.Date1 <= now && n.Date2 >= now, stoppingToken);

                            var subEquipment = await dbContext.SubEquipments.FindAsync(subEquipmentId);
                            if (subEquipment != null)
                            {
                                string newStatus = isInMaintenance ? "en_maintenance" : "bon_etat";

                                if (subEquipment.Status != newStatus) // Update only if status changes
                                {
                                    subEquipment.Status = newStatus;
                                    _logger.LogInformation($"Updated SubEquipment ID {subEquipment.Id} status to '{newStatus}'.");
                                }
                            }
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating sub-equipment status.");
                }

                // Wait before checking again
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
