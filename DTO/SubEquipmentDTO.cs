using sp_backend.Models;

namespace WeatherApi.DTOs
{
    public class SubEquipmentDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Cycle { get; set; }
        public string? Status { get; set; }
        public DateTime CreationDate { get; set; }
        public int? EquipmentId { get; set; } // The foreign key
        public List<Maintenance> Maintenances { get; set; } = new();
    }
}