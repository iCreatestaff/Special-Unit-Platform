using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.DTOs;

namespace sp_backend.DTO
{
    public class EquipmentWithQuantityDto
    {
        public EquipmentDto Equipment { get; set; } = null!;
        public int Quantity { get; set; }
    }
}