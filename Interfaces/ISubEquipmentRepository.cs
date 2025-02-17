using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.Models;

namespace sp_backend.Interfaces
{
    public interface ISubEquipmentRepository
    {
        Task<SubEquipment> GetSubEquipmentByIdAsync(int id);
        Task<IEnumerable<SubEquipment>> GetAllSubEquipmentsAsync();
        Task<SubEquipment> CreateSubEquipmentAsync(SubEquipment subEquipment);
        Task<SubEquipment> UpdateSubEquipmentAsync(SubEquipment subEquipment);
        Task DeleteSubEquipmentAsync(int id);
        Task UpdateSubEquipmentAsync(int id, SubEquipment subEquipment);
    }

}