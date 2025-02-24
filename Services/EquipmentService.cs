using WeatherApi.Interfaces;
using WeatherApi.Models;
using Microsoft.EntityFrameworkCore;
using WeatherApi.DTOs;

namespace WeatherApi.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly AppDbContext _context;

        public EquipmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Equipment>> GetAllEquipmentsAsync()
        {
            return await _context.Equipments.Include(e => e.SubEquipments).ToListAsync();
        }

        public async Task<Equipment?> GetEquipmentByIdAsync(int id)
        {
            return await _context.Equipments.Include(e => e.SubEquipments)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<EquipmentDto>> GetAvailableEquipmentAsync()
        {
            return await _context.Equipments
                .Where(e => (bool)e.Availability) // Fetch only available equipment
                .Select(e => new EquipmentDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Availability = e.Availability,
                    Type = e.Type,
                    Photo = e.Photo,
                    SubEquipments = e.SubEquipments.Select(se => new SubEquipmentDto
                    {
                        Id = se.Id,
                        Name = se.Name,
                        Status = se.Status
                    }).ToList()
                })
                .ToListAsync();
        }


        public async Task<Equipment> CreateEquipmentAsync(Equipment equipment)
        {
            // Find the corresponding EquipmentStock by EquipmentName
            var equipmentStock = await _context.EquipmentStocks
                .FirstOrDefaultAsync(es => es.EquipmentName == equipment.Name);

            if (equipmentStock != null)
            {
                // Increase the stock quantity
                equipmentStock.Quantity += 1;

                // Assign the EquipmentStockId to the new Equipment
                equipment.EquipmentStockId = equipmentStock.Id;

                // Add the equipment to the list of equipments
                equipmentStock.Equipments.Add(equipment);
            }
            else
            {
                // Optionally, throw an error or create a new EquipmentStock
                throw new Exception("No matching EquipmentStock found for this equipment.");
            }

            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();

            return equipment;
        }


        public async Task<Equipment?> UpdateEquipmentAsync(int id, Equipment equipment)
        {
            var existingEquipment = await _context.Equipments.FindAsync(id);
            if (existingEquipment == null)
            {
                return null;
            }

            existingEquipment.Name = equipment.Name ?? existingEquipment.Name;
            existingEquipment.Availability = equipment.Availability ?? existingEquipment.Availability;
            existingEquipment.Type = equipment.Type ?? existingEquipment.Type;

            await _context.SaveChangesAsync();
            return existingEquipment;
        }



        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null)
            {
                return false;
            }

            // Find the corresponding EquipmentStock
            var equipmentStock = await _context.EquipmentStocks
                .FirstOrDefaultAsync(es => es.Id == equipment.EquipmentStockId);

            if (equipmentStock != null)
            {
                // Decrease the stock quantity
                equipmentStock.Quantity = Math.Max(0, equipmentStock.Quantity - 1);

                // Remove the equipment from the list
                equipmentStock.Equipments.Remove(equipment);
            }

            _context.Equipments.Remove(equipment);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
