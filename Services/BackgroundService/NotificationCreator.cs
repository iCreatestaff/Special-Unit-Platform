using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sp_backend_March4.Models;
using WeatherApi;

namespace sp_backend_March4.Services.NotificationCreator
{
    public class NotificationCreator : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationCreator> _logger;
        private readonly TimeZoneInfo _localTimeZone;

        public NotificationCreator(IServiceProvider serviceProvider, ILogger<NotificationCreator> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var tunisiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
                var currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tunisiaTimeZone);
                var thresholdTime = currentTime.AddMinutes(30);


                var maintenances = await dbContext.Maintenances
                    .Where(m => m.Status == "Scheduled" &&
                                m.MaintenanceDate <= thresholdTime &&
                                m.MaintenanceDate > currentTime)
                    .ToListAsync(stoppingToken);

                foreach (var maintenance in maintenances)
                {
                    bool alreadyNotified = await dbContext.Notifications
                        .AnyAsync(n =>
                            n.Type == "maintenance" &&
                            n.Details!.Contains($"Maintenance ID: {maintenance.Id}"),
                            stoppingToken);

                    if (!alreadyNotified)
                    {
                        var notification = new Notification
                        {
                            Type = "maintenance",
                            Details = $"Maintenance ID: {maintenance.Id} scheduled at {maintenance.MaintenanceDate} is due in 30 minutes."
                        };

                        dbContext.Notifications.Add(notification);
                    }
                }

                await dbContext.SaveChangesAsync(stoppingToken);

                // Run this check every 5 minutes
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}