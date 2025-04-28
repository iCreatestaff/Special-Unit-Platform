using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sp_backend_March4.Models
{
    public class Notification
    {
        public int Id { get; set; }

        // Type of notification (e.g., "maintenance", "mission", "alert", etc.)
        public string Type { get; set; } = string.Empty;

        // Whether the notification has been read
        public bool IsRead { get; set; } = false;

        // Main content or message of the notification
        public string Details { get; set; } = string.Empty;

        // Who this notification is for (Admin, Agent, etc.)
        public int? RecipientId { get; set; }  // Optional foreign key

        // Optional: Link to entity causing the notification (maintenanceId, missionId, etc.)
        public int? ReferenceId { get; set; }

        // Timestamp to sort notifications
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow + TimeSpan.FromHours(1);
    }

}