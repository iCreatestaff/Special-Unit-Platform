using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sp_backend.Models
{
    public class AccountMission
    {
        public int AccountId { get; set; }
        public Account Account { get; set; }

        public int MissionId { get; set; }
        public Mission Mission { get; set; }

        public DateTime AssignedDate { get; set; }
    }

}