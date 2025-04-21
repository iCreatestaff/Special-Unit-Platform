using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sp_backend_March4.DTO
{
    // For creating a new notification
    public class CreateNotificationDto
    {
        public string Type { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public int? RecipientId { get; set; }
        public int? ReferenceId { get; set; }
    }

    // For returning a notification to the client
    public class NotificationDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public string Details { get; set; } = string.Empty;
        public int? RecipientId { get; set; }
        public int? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}