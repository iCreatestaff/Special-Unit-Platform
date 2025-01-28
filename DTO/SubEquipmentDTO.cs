namespace WeatherApi.DTOs
{
    public class SubEquipmentDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Cycle { get; set; }
        public bool? Status { get; set; }
        public int? EquipmentId { get; set; } // The foreign key
    }
}
