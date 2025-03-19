using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sp_backend.Models;

namespace sp_backend.DTO
{
    public class MaintenanceDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Type { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int? SubEquipmentId { get; set; }
        public DateTime MaintenanceDate { get; set; } = DateTime.UtcNow;
        public ICollection<Item>? Items { get; set; }
    }

    public class GroupedMaintenanceDto
    {
        public string Name { get; set; } // Maintenance Description
        public List<MaintenanceDTO> Maintenances { get; set; }
    }

}