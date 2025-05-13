using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sp_backend_March4.DTO;
using sp_backend_March4.Interfaces;
using sp_backend_March4.Models;
using WeatherApi;

namespace sp_backend_March4.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
        {
            return await _context.Notifications
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    Details = n.Details,
                    CreatedAt = n.CreatedAt,
                    RecipientId = n.RecipientId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<NotificationDto>> GetByTypeAsync(string type)
        {
            return await _context.Notifications
                .Where(n => n.Type == type)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Type = n.Type,
                    Details = n.Details,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    RecipientId = n.RecipientId,
                    ReferenceId = n.ReferenceId
                })
                .ToListAsync();
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                Type = dto.Type,
                Details = dto.Details,
                RecipientId = dto.RecipientId,
                ReferenceId = dto.ReferenceId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return ToDto(notification);
        }

        public async Task<List<NotificationDto>> GetNotificationsByRecipientIdAsync(int recipientId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.RecipientId == recipientId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(ToDto).ToList();
        }

        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null) return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        private static NotificationDto ToDto(Notification n) => new NotificationDto
        {
            Id = n.Id,
            Type = n.Type,
            IsRead = n.IsRead,
            Details = n.Details,
            RecipientId = n.RecipientId,
            ReferenceId = n.ReferenceId,
            CreatedAt = n.CreatedAt
        };
    }

}