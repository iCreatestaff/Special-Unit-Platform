using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sp_backend.DTO
{
    public class MaintenanceDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; } = string.Empty;
        public string? Type { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int? SubEquipmentId { get; set; }
        public List<ItemDTO>? Items { get; set; } = new();
    }

}