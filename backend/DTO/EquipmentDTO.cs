using sp_backend.Models;

namespace WeatherApi.DTOs
{
    public class EquipmentDto
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
