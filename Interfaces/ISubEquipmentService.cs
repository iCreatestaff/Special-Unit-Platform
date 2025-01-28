using WeatherApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherApi.Interfaces
{
    public interface ISubEquipmentService
    {
        Task<IEnumerable<SubEquipment>> GetAllSubEquipmentsAsync();
        Task<SubEquipment?> GetSubEquipmentByIdAsync(int id);
        Task<SubEquipment> CreateSubEquipmentAsync(SubEquipment subEquipment);
        Task<SubEquipment?> UpdateSubEquipmentAsync(int id, SubEquipment subEquipment);
        Task<bool> DeleteSubEquipmentAsync(int id);
    }
}
