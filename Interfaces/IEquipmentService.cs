using WeatherApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApi.DTOs;

namespace WeatherApi.Interfaces
{
    public interface IEquipmentService
    {
        Task<IEnumerable<Equipment>> GetAllEquipmentsAsync();
        Task<Equipment?> GetEquipmentByIdAsync(int id);
        Task<Equipment> CreateEquipmentAsync(Equipment equipment);
        Task<Equipment?> UpdateEquipmentAsync(int id, Equipment equipment);
        Task<bool> DeleteEquipmentAsync(int id);
        Task<List<EquipmentDto>> GetAvailableEquipmentAsync();
    }
}
