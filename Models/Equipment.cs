using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sp_backend.Models;

namespace WeatherApi.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        public string? Name { get; set; } = string.Empty;

        public bool? Availability { get; set; } = true;

        public string? Type { get; set; } = string.Empty;

        public ICollection<SubEquipment>? SubEquipments { get; set; }

        public List<EquipmentMission> EquipmentMissions { get; set; } = new();

        public List<Nonavailability> Nonavailabilities { get; set; } = new();

        // public int EquipmentStockId { get; set; }

        /*  [ForeignKey(nameof(EquipmentStockId))]
          public EquipmentStock EquipmentStock { get; set; } = null!;
  */
        public string? Photo { get; set; } // Store the file path or URL of the photo
    }
}
