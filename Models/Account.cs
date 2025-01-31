using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using sp_backend.Models;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Username { get; set; }

    [Required]
    public string PasswordHash { get; set; } // Store hashed passwords

    [Required]
    [MaxLength(20)]
    public string Role { get; set; } // "SuperAdmin", "Admin", "Agent"

    // Additional details
    public string? SocialFile { get; set; }
    public string? MedicalFile { get; set; }
    public string? CareerFile { get; set; }

    // Relationships
    public List<Nonavailability>? Nonavailabilities { get; set; } = new List<Nonavailability>();

    public ICollection<Mission> Missions { get; set; } = new List<Mission>();
}
