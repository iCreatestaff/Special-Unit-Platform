using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.DTOs;

namespace sp_backend.DTO
{
    public class EquipmentResponseDTO
    {
        public int Id { get; set; }  // Add the Id property
        public string? Name { get; set; }
        public bool? Availability { get; set; }
        public string? Type { get; set; }
        public string? Photo { get; set; }
        public int EquipmentStockId { get; set; }
        public List<SubEquipmentDto>? SubEquipments { get; set; }
    }
}