using Microsoft.EntityFrameworkCore;
using sp_backend.DTO;
using sp_backend.Models;
using WeatherApi;
using WeatherApi.Models;

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
                    Quantity = es.Quantity,
                    Equipments = es.Equipments
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

        public async Task<List<Equipment>> UpdateEquipmentsByEquipmentStockIdAsync(int equipmentStockId, Equipment updatedEquipment)
        {
            // Find the EquipmentStock by its ID
            var equipmentStock = await _context.EquipmentStocks
                .Include(es => es.Equipments) // Include related Equipment objects
                .FirstOrDefaultAsync(es => es.Id == equipmentStockId);

            if (equipmentStock == null)
            {
                throw new Exception("EquipmentStock not found.");
            }

            // Update only non-null values in Equipments
            foreach (var equipment in equipmentStock.Equipments)
            {
                if (!string.IsNullOrEmpty(updatedEquipment.Type))
                {
                    equipment.Type = updatedEquipment.Type;
                }

                if (updatedEquipment.Availability.HasValue) // Check for nullable boolean
                {
                    equipment.Availability = updatedEquipment.Availability;
                }

                if (updatedEquipment.Photo != null) // Check if Photo is provided
                {
                    equipment.Photo = updatedEquipment.Photo;
                }
                if (updatedEquipment.SubEquipments != null) // Check if Photo is provided
                {
                    equipment.SubEquipments = updatedEquipment.SubEquipments;
                }

                // Add more fields if necessary
            }

            // Save changes
            await _context.SaveChangesAsync();

            return equipmentStock.Equipments;
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
