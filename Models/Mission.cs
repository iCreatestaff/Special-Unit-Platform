using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WeatherApi.Models;

namespace sp_backend.Models
{
    public class Mission
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending";

        [ForeignKey("Admin")]
        public int AdminId { get; set; }

        // Many-to-many relationship with Account
        public List<Account> AssignedAccounts { get; set; } = new();

        // Many-to-many relationship with Equipment
        public List<Equipment> AssignedEquipment { get; set; } = new();

        public List<AccountMission> AccountMissions { get; set; } = new();
        public List<EquipmentMission> EquipmentMissions { get; set; } = new();
    }
}
