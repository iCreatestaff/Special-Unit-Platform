using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sp_backend_March4.DTO;

namespace sp_backend_March4.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync();
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);
        Task<List<NotificationDto>> GetNotificationsByRecipientIdAsync(int recipientId);
        Task<IEnumerable<NotificationDto>> GetByTypeAsync(string type);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> DeleteNotificationAsync(int notificationId);
    }

}