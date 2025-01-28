using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        
        public string? Name { get; set; } = string.Empty;

        public bool? Availability { get; set; }

        public string? Type { get; set; } = string.Empty;
        
        public ICollection<SubEquipment>? SubEquipments { get; set; }
    }
}
