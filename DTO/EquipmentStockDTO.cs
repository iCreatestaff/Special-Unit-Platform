using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.Models;

namespace sp_backend.DTO
{
    public class EquipmentStockDTO
    {
        public int Id { get; set; }
        public string EquipmentName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string? Photo { get; set; }
        public List<Equipment> Equipments { get; set; } = new();
    }
}