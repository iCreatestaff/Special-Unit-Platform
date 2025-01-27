namespace WeatherApi.Models  
{  
    public class Equipment  
    {  
        public int Id { get; set; } // Unique identifier for the equipment  
        public string Name { get; set; } // Name of the equipment  
        public bool Availability { get; set; } // Availability status of the equipment  
        public string Type { get; set; } // Type of the equipment (e.g., "Heavy", "Light", etc.)  
        
        // List of sub-equipment associated with this equipment  
        public List<SubEquipment> SubEquipments { get; set; } = new List<SubEquipment>(); 
    }  
}