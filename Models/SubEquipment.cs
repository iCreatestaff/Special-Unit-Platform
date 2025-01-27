namespace WeatherApi.Models // Adjust the namespace according to your project structure  
{  
    public class SubEquipment  
    {  
        public int Id { get; set; } // Unique identifier for the sub-equipment  
        public string Name { get; set; } // Name of the sub-equipment  
        public string Cycle { get; set; } // Optional description of the sub-equipment  
        public bool Status { get; set; } // Availability status of the sub-equipment 
        // Foreign key property  
        public int EquipmentId { get; set; } // Identifier for the parent Equipment  
        
        // Navigation property  
        public Equipment Equipment { get; set; }  
    }  
}