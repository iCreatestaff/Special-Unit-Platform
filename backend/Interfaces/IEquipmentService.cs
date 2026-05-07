using WeatherApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApi.DTOs;
using sp_backend.DTO;

namespace WeatherApi.Interfaces
{
    public interface IEquipmentService
    {
        Task<List<EquipmentResponseDTO>> GetAllEquipmentsAsync();
        Task<Equipment?> GetEquipmentByIdAsync(int id);
        Task<Equipment> CreateEquipmentAsync(Equipment equipment);
        Task<List<Equipment>> CreateEquipmentWithQuantityAsync(Equipment equipment, int quantity);
        Task<Equipment?> UpdateEquipmentAsync(int id, Equipment equipment);
        Task<bool> DeleteEquipmentAsync(int id);
        Task<List<Equipment>> GetAvailableEquipmentAsync(DateTime d1, DateTime d2);
    }
}
