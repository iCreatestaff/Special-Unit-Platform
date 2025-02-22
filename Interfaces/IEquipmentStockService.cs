using System.Collections.Generic;
using System.Threading.Tasks;
using sp_backend.DTO;

namespace sp_backend.Services
{
    public interface IEquipmentStockService
    {
        Task<IEnumerable<EquipmentStockDTO>> GetAllAsync();
        Task<EquipmentStockDTO?> GetByIdAsync(int id);
        Task<bool> AddAsync(EquipmentStockDTO equipmentStockDto);
        Task<bool> UpdateAsync(int id, EquipmentStockDTO equipmentStockDto);
        Task<bool> DeleteAsync(int id);
    }
}
