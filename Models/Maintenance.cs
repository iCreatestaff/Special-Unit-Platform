using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
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

        public ICollection<Item>? Items { get; set; }
        public DateTime MaintenanceDate { get; set; } = DateTime.UtcNow;

        public int? SubEquipmentId { get; set; }

        [ForeignKey("SubEquipmentId")]
        public SubEquipment? SubEquipment { get; set; }

    }
}