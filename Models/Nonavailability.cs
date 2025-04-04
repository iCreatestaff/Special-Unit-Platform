using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using sp_backend.Models;
using WeatherApi.Models;

public class Nonavailability
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date1 { get; set; }

    [Required]
    public DateTime Date2 { get; set; }

    public string? Type { get; set; }  // Values: "Account" or "SubEquipment"
    public string? Reason { get; set; }

    public int? MissionID { get; set; }

    [ForeignKey(nameof(MissionID))]
    public Mission? Mission { get; set; }

    // Foreign key reference to Account (nullable)
    public int? AccountId { get; set; }

    [ForeignKey("AccountId")]
    public Account? Account { get; set; }

    // Foreign key reference to SubEquipment (nullable)
    public int? EquipmentId { get; set; }

    [ForeignKey("EquipmentId")]
    public Equipment? Equipment { get; set; }
}
