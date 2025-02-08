using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Models
{
    public class SubEquipment
    {
        public int Id { get; set; }


        public string? Name { get; set; } = string.Empty;

        public string? Cycle { get; set; } // Optional, nullable field

        public string? Status { get; set; }

        public int? EquipmentId { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public Equipment? Equipment { get; set; }
    }
}
