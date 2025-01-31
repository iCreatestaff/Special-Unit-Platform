using System;
using System.Collections.Generic;

namespace sp_backend.DTO
{
    public class MissionCreateDTO
    {
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public int AdminId { get; set; }
        public List<int> AssignedAccountIds { get; set; } = new List<int>();
        public List<int> AssignedEquipmentIds { get; set; } = new List<int>();
    }
}
