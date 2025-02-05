using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.Models;

namespace sp_backend.Models
{
    public class EquipmentMission
    {
        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public int MissionId { get; set; }
        public Mission Mission { get; set; }
    }

}