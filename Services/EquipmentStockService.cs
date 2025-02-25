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
                    Photo = es.Photo,
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
                    Quantity = es.Quantity,
                    Photo = es.Photo

                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AddAsync(EquipmentStockDTO equipmentStockDto)
        {
            var equipmentStock = new EquipmentStock
            {
                EquipmentName = equipmentStockDto.EquipmentName,
                Quantity = equipmentStockDto.Quantity,
                Photo = equipmentStockDto.Photo

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
            existing.Photo = equipmentStockDto.Photo;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Equipment>> UpdateEquipmentsByEquipmentStockIdAsync(int equipmentStockId, Equipment updatedEquipment)
        {
            // Find the EquipmentStock by its ID
            var equipmentStock = await _context.EquipmentStocks
     .Include(es => es.Equipments)
         .ThenInclude(e => e.SubEquipments)
     .AsSplitQuery() // 👈 This tells EF Core to split queries instead of using a single large query
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

                if (!string.IsNullOrEmpty(updatedEquipment.Name)) // Check if Name is provided
                {
                    equipment.Name = updatedEquipment.Name;
                    equipmentStock.EquipmentName = updatedEquipment.Name; // Also update EquipmentStock name
                }

                if (!string.IsNullOrEmpty(updatedEquipment.Photo)) // Check if Photo is provided
                {
                    equipment.Photo = updatedEquipment.Photo;
                }

                // **Replace all SubEquipments with the new ones**
                if (updatedEquipment.SubEquipments != null)
                {
                    // **Step 1: Remove all existing SubEquipments**
                    _context.SubEquipments.RemoveRange(equipment.SubEquipments);

                    // **Step 2: Assign the new SubEquipments**
                    equipment.SubEquipments = updatedEquipment.SubEquipments.Select(se => new SubEquipment
                    {
                        Name = se.Name,
                        Cycle = se.Cycle,
                        Status = se.Status,
                        CreationDate = DateTime.UtcNow,
                        EquipmentId = equipment.Id // Maintain the relationship
                    }).ToList();
                }
            }

            // Save changes
            await _context.SaveChangesAsync();

            return equipmentStock.Equipments;
        }


        public async Task<bool> UpdateSubEquipmentByNameAsync(int equipmentStockId, string subEquipmentName, SubEquipment updatedSubEquipment)
        {
            // Fetch the EquipmentStock with related Equipments and SubEquipments using Query Splitting
            var equipmentStock = await _context.EquipmentStocks
                .Where(es => es.Id == equipmentStockId)
                .Include(es => es.Equipments)
                    .ThenInclude(e => e.SubEquipments)
                .AsSplitQuery() // ⚡️ This solves the query performance issue!
                .FirstOrDefaultAsync();

            if (equipmentStock == null || !equipmentStock.Equipments.Any())
            {
                return false; // No matching EquipmentStock or Equipments found
            }

            bool isUpdated = false;
            foreach (var equipment in equipmentStock.Equipments)
            {
                foreach (var subEquipment in equipment.SubEquipments.Where(se => se.Name == subEquipmentName))
                {
                    isUpdated = true;

                    if (!string.IsNullOrEmpty(updatedSubEquipment.Name))
                        subEquipment.Name = updatedSubEquipment.Name;

                    if (!string.IsNullOrEmpty(updatedSubEquipment.Cycle))
                        subEquipment.Cycle = updatedSubEquipment.Cycle;

                    if (!string.IsNullOrEmpty(updatedSubEquipment.Status))
                        subEquipment.Status = updatedSubEquipment.Status;
                }
            }

            if (isUpdated)
            {
                await _context.SaveChangesAsync();
            }

            return isUpdated;
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
