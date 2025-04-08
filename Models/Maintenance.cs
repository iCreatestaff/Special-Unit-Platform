using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using sp_backend_March4.Models;
using WeatherApi.Models;

namespace sp_backend.Models
{
    public class Maintenance
    {
        [Key]
        public int Id { get; set; }

        public string? Name { get; set; } = string.Empty;

        public string? Type { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;
        public string? Cycle { get; set; }

        public ICollection<Item>? Items { get; set; }
        public DateTime MaintenanceDate { get; set; } = DateTime.UtcNow;
        public DateTime MaintenanceEndDate { get; set; }

        public int? SubEquipmentId { get; set; }
        public RequestMaintenance? RequestMaintenance { get; set; }

        [ForeignKey("SubEquipmentId")]
        public SubEquipment? SubEquipment { get; set; }

    }
}