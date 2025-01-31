using System;
using System.Collections.Generic;

namespace sp_backend.DTO
{
    public class MissionDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public int AdminId { get; set; }
        public List<int> AssignedAccountIds { get; set; } = new();
        public List<int> AssignedEquipmentIds { get; set; } = new();
    }
}
