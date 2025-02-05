using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sp_backend.DTO
{
    public class AccountMissionDTO
    {
        public int MissionId { get; set; }
        public int AccountId { get; set; }
        public DateTime AssignedDate { get; set; } // Ensure AccountMission has this field
    }
}
