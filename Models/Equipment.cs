using System.ComponentModel.DataAnnotations;
using sp_backend.Models;

namespace WeatherApi.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        public string? Name { get; set; } = string.Empty;

        public bool? Availability { get; set; }

        public string? Type { get; set; } = string.Empty;

        public ICollection<SubEquipment>? SubEquipments { get; set; }

        public List<Mission> Missions { get; set; } = new List<Mission>();

        // New attributes
        public string Status { get; set; } = "Normal"; // Default status

        public string? Photo { get; set; } // Store the file path or URL of the photo
    }
}
