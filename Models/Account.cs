using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using sp_backend.Models;
using sp_backend_March4.Models;

public class Account
{

    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public string Username { get; set; }
    public string Type { get; set; }
    public string Badge { get; set; }

    public string? PasswordHash { get; set; }

    [Required]
    [MaxLength(20)]
    public string Role { get; set; }

    // Additional details
    public string? SocialFile { get; set; }
    public string? MedicalFile { get; set; }
    public string? CareerFile { get; set; }
    public string? Photo { get; set; }

    // Relationships
    public List<Nonavailability> Nonavailabilities { get; set; } = new();

    // Many-to-many relationship with Mission

    public List<AccountMission> AccountMissions { get; set; } = new();
    public List<AccountTraining> AccountTrainings { get; set; }
    public ICollection<MessageAgent> SentMessages { get; set; } = new List<MessageAgent>();
    public ICollection<MessageAgent> ReceivedMessages { get; set; } = new List<MessageAgent>();
}
