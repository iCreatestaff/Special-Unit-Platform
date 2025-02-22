using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.Models;

namespace sp_backend.Models
{
    public class EquipmentStock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? EquipmentName { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        public List<Equipment> Equipments { get; set; } = new();
    }
}
