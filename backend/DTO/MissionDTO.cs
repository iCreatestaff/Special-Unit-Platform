using System;
using System.Collections.Generic;

namespace sp_backend.DTO
{
    public class MissionDTO
    {
        public int Id { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";

        public List<int> AssignedAccounts { get; set; } = new();

        // Many-to-many relationship with Equipment
        public List<int> AssignedEquipments { get; set; } = new();
        public int AdminId { get; set; }
    }
}
