using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using sp_backend.Models;

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
        [JsonIgnore]
        public Equipment? Equipment { get; set; }
        public List<Nonavailability> Nonavailabilities { get; set; } = new();
        public List<Maintenance> Maintenances { get; set; } = new();
    }
}
