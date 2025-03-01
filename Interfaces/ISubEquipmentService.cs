using WeatherApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApi.DTOs;

namespace WeatherApi.Interfaces
{
    public interface ISubEquipmentService
    {
        Task<IEnumerable<SubEquipmentDto>> GetAllSubEquipmentsAsync();
        Task<SubEquipmentDto?> GetSubEquipmentByIdAsync(int id);
        Task<SubEquipment> CreateSubEquipmentAsync(SubEquipment subEquipment);
        Task<SubEquipment?> UpdateSubEquipmentAsync(int id, SubEquipment subEquipment);
        Task<bool> DeleteSubEquipmentAsync(int id);
    }
}
