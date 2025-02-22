using Microsoft.EntityFrameworkCore;
using sp_backend.DTO;
using sp_backend.Models;
using WeatherApi;

namespace sp_backend.Services
{
    public class EquipmentStockService : IEquipmentStockService
    {
        private readonly WeatherApi.AppDbContext _context;

        public EquipmentStockService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EquipmentStockDTO>> GetAllAsync()
        {
            return await _context.EquipmentStocks
                .Select(es => new EquipmentStockDTO
                {
                    Id = es.Id,
                    EquipmentName = es.EquipmentName,
                    Quantity = es.Quantity
                })
                .ToListAsync();
        }

        public async Task<EquipmentStockDTO?> GetByIdAsync(int id)
        {
            return await _context.EquipmentStocks
                .Where(es => es.Id == id)
                .Select(es => new EquipmentStockDTO
                {
                    Id = es.Id,
                    EquipmentName = es.EquipmentName,
                    Quantity = es.Quantity
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddAsync(EquipmentStockDTO equipmentStockDto)
        {
            var equipmentStock = new EquipmentStock
            {
                EquipmentName = equipmentStockDto.EquipmentName,
                Quantity = equipmentStockDto.Quantity
            };

            _context.EquipmentStocks.Add(equipmentStock);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int id, EquipmentStockDTO equipmentStockDto)
        {
            var existing = await _context.EquipmentStocks.FindAsync(id);
            if (existing == null) return false;

            existing.EquipmentName = equipmentStockDto.EquipmentName;
            existing.Quantity = equipmentStockDto.Quantity;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var equipmentStock = await _context.EquipmentStocks.FindAsync(id);
            if (equipmentStock == null) return false;

            _context.EquipmentStocks.Remove(equipmentStock);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
