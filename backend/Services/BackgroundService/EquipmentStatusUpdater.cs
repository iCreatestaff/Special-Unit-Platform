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
    public class EquipmentStatusUpdater : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<EquipmentStatusUpdater> _logger;
        private readonly TimeZoneInfo _localTimeZone;

        public EquipmentStatusUpdater(IServiceScopeFactory serviceScopeFactory, ILogger<EquipmentStatusUpdater> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _localTimeZone = TimeZoneInfo.CreateCustomTimeZone("UTC+1", TimeSpan.FromHours(1), "UTC+1", "UTC+1");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EquipmentStatusUpdater Service is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _localTimeZone);
                        _logger.LogInformation($"Current Local Time: {now}");

                        // Get all equipment with sub-equipment included
                        var allEquipment = await dbContext.Equipments
                            .Include(e => e.SubEquipments) // Load sub-equipment
                            .ToListAsync(stoppingToken);

                        foreach (var equipment in allEquipment)
                        {
                            _logger.LogInformation($"Checking Equipment ID {equipment.Id}");

                            // Check if the equipment is in a nonavailability period
                            bool isUnavailableDueToNonavailability = await dbContext.Nonavailabilities
                                .AnyAsync(n => n.EquipmentId == equipment.Id && n.Date1 <= now && n.Date2 >= now, stoppingToken);

                            // Check if any sub-equipment is 'en_panne'
                            bool isUnavailableDueToSubEquipment = equipment.SubEquipments
                                .Any(sub => sub.Status.ToLower() == "en_panne");

                            bool newAvailability = !(isUnavailableDueToNonavailability || isUnavailableDueToSubEquipment);

                            if (equipment.Availability != newAvailability) // Update only if it changes
                            {
                                _logger.LogInformation($"Updating Equipment ID {equipment.Id} availability to '{newAvailability}'");

                                equipment.Availability = newAvailability;
                                dbContext.Equipments.Update(equipment); // Ensure EF tracks the change
                            }
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Equipment availability updates saved successfully.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while updating equipment availability.");
                }

                // Wait before checking again
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
