using WeatherApi.Interfaces;
using WeatherApi.Models;
using Microsoft.EntityFrameworkCore;
using WeatherApi.DTOs;
using sp_backend.DTO;
using AutoMapper;

namespace WeatherApi.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public EquipmentService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<EquipmentResponseDTO>> GetAllEquipmentsAsync()
        {
            var equipments = await _context.Equipments
                .Include(e => e.SubEquipments)
                .ToListAsync();

            return _mapper.Map<List<EquipmentResponseDTO>>(equipments);
        }

        public async Task<Equipment?> GetEquipmentByIdAsync(int id)
        {
            return await _context.Equipments.Include(e => e.SubEquipments)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Equipment>> GetAvailableEquipmentAsync(DateTime d1, DateTime d2)
        {
            return await _context.Equipments
                .Where(e => !e.Nonavailabilities.Any(n =>
                    (d1 >= n.Date1 && d1 <= n.Date2) ||  // d1 falls within a non-availability range
                    (d2 >= n.Date1 && d2 <= n.Date2) ||  // d2 falls within a non-availability range
                    (n.Date1 >= d1 && n.Date2 <= d2)     // non-availability is fully within the requested period
                ))
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

        public async Task<List<Equipment>> CreateEquipmentWithQuantityAsync(Equipment equipment, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero.");
            }

            var equipmentStock = await _context.EquipmentStocks
                .FirstOrDefaultAsync(es => es.EquipmentName == equipment.Name);

            if (equipmentStock == null)
            {
                throw new Exception("No matching EquipmentStock found for this equipment.");
            }

            var createdEquipments = new List<Equipment>();

            for (int i = 0; i < quantity; i++)
            {
                var newEquipment = new Equipment
                {
                    Name = equipment.Name,
                    Type = equipment.Type,
                    Availability = equipment.Availability,
                    EquipmentStockId = equipmentStock.Id,
                    Photo = equipment.Photo,
                    SubEquipments = equipment.SubEquipments
                        .Select(se => new SubEquipment
                        {
                            Name = se.Name,
                            Cycle = se.Cycle,
                            Status = se.Status,
                            CreationDate = DateTime.UtcNow
                        })
                        .ToList() // Ensure a new list is created
                };

                _context.Equipments.Add(newEquipment);
                createdEquipments.Add(newEquipment);
            }

            // Update stock quantity once after all insertions
            equipmentStock.Quantity += quantity;

            await _context.SaveChangesAsync();

            return createdEquipments;
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
            existingEquipment.Photo = equipment.Photo ?? existingEquipment.Photo;

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
