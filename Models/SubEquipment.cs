using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Models
{
    public class SubEquipment
    {
        public int Id { get; set; }


        public string? Name { get; set; } = string.Empty;

        public string? Cycle { get; set; } // Optional, nullable field

        public bool? Status { get; set; }

        public int? EquipmentId { get; set; }
        
        
        public Equipment? Equipment { get; set; }
    }
}
