using System.Collections.Generic;
using System.Threading.Tasks;
using sp_backend.DTO;
using WeatherApi.Models;

namespace sp_backend.Services
{
    public interface IEquipmentStockService
    {
        Task<IEnumerable<EquipmentStockDTO>> GetAllAsync();
        Task<EquipmentStockDTO?> GetByIdAsync(int id);
        Task<bool> AddAsync(EquipmentStockDTO equipmentStockDto);
        Task<List<Equipment>> UpdateEquipmentsByEquipmentStockIdAsync(int equipmentStockId, Equipment updatedEquipment);
        Task<bool> AddSubEquipmentToAllEquipmentsAsync(int equipmentStockId, SubEquipment newSubEquipment);

        Task<bool> UpdateSubEquipmentByNameAsync(int equipmentStockId, string subEquipmentName, SubEquipment updatedSubEquipment);
        Task<bool> DeleteSubEquipmentFromAllEquipmentsAsync(int equipmentStockId, string subEquipmentName);
        Task<bool> UpdateAsync(int id, EquipmentStockDTO equipmentStockDto);
        Task<bool> DeleteAsync(int id);
    }
}
