using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sp_backend.DTO
{
    namespace sp_backend.DTO
    {
        public class EquipmentMissionResponseDTO
        {
            public int EquipmentId { get; set; }
            public string EquipmentName { get; set; }
            public string EquipmentStatus { get; set; }
            public string EquipmentType { get; set; }

            public int MissionId { get; set; }
            public string MissionDescription { get; set; }
            public string MissionLocation { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string MissionStatus { get; set; }
        }
    }

}