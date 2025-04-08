using WeatherApi.Interfaces;
using WeatherApi.Models;
using Microsoft.EntityFrameworkCore;
using WeatherApi.DTOs;
using sp_backend.DTO;
using AutoMapper;
using sp_backend.Models;

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
            var equipment = await _context.Equipments
                .Include(e => e.SubEquipments)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipment != null && equipment.SubEquipments != null)
            {
                foreach (var sub in equipment.SubEquipments)
                {
                    if (string.IsNullOrEmpty(sub.Status))
                    {
                        sub.Status = "bon_etat";
                    }
                }

                await _context.SaveChangesAsync(); // Persist changes to DB
            }

            return equipment;
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
            }
            else
            {
                throw new Exception("No matching EquipmentStock found for this equipment.");
            }

            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync(); // Save Equipment first to get an ID

            var maintenancesToAdd = new List<Maintenance>();

            foreach (var subEquipment in equipment.SubEquipments)
            {
                subEquipment.EquipmentId = equipment.Id; // Link it to the Equipment
                subEquipment.Id = 0; // Ensure Id is NOT explicitly set (EF will generate it)
                _context.SubEquipments.Add(subEquipment);
                await _context.SaveChangesAsync(); // Save each SubEquipment to get an ID

                // Compute Maintenance Date
                DateTime scheduledDate = ComputeMaintenanceDate(subEquipment.Cycle);

                // Create Maintenance for the SubEquipment
                var maintenance = new Maintenance
                {
                    Description = $"Initial for {subEquipment.Name}",
                    MaintenanceDate = scheduledDate,
                    SubEquipmentId = subEquipment.Id
                };

                maintenancesToAdd.Add(maintenance);
            }

            // Add all maintenances in one go for better efficiency
            await _context.Maintenances.AddRangeAsync(maintenancesToAdd);
            await _context.SaveChangesAsync(); // Save all Maintenances

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
            var allMaintenances = new List<Maintenance>();

            for (int i = 0; i < quantity; i++)
            {
                var newEquipment = new Equipment
                {
                    Name = equipment.Name,
                    Type = equipment.Type,
                    Availability = equipment.Availability,
                    EquipmentStockId = equipmentStock.Id,
                    Photo = equipment.Photo
                };

                _context.Equipments.Add(newEquipment);
                await _context.SaveChangesAsync(); // Ensure Equipment ID is generated

                var newSubEquipments = new List<SubEquipment>();

                foreach (var subEquipment in equipment.SubEquipments)
                {
                    var newSubEquipment = new SubEquipment
                    {
                        Name = subEquipment.Name,
                        Cycle = subEquipment.Cycle,
                        Status = subEquipment.Status,
                        CreationDate = DateTime.UtcNow,
                        EquipmentId = newEquipment.Id // Assign Equipment ID
                    };

                    newSubEquipments.Add(newSubEquipment);
                }

                // Add all subequipments in bulk
                await _context.SubEquipments.AddRangeAsync(newSubEquipments);
                await _context.SaveChangesAsync(); // Ensure SubEquipment IDs are generated

                // Create Maintenance records for each SubEquipment
                foreach (var subEquipment in newSubEquipments)
                {
                    var maintenance = new Maintenance
                    {
                        Description = $"Initial maintenance for {subEquipment.Name}",
                        MaintenanceDate = ComputeMaintenanceDate(subEquipment.Cycle),
                        SubEquipmentId = subEquipment.Id
                    };
                    allMaintenances.Add(maintenance);
                }

                createdEquipments.Add(newEquipment);
            }

            // Add all maintenances in one batch
            await _context.Maintenances.AddRangeAsync(allMaintenances);
            await _context.SaveChangesAsync();

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

        private DateTime ComputeMaintenanceDate(string? cycle)
        {
            if (string.IsNullOrWhiteSpace(cycle))
            {
                return DateTime.UtcNow; // Default to today if cycle is invalid
            }

            var parts = cycle.Split('-');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int number))
            {
                return DateTime.UtcNow; // Default to today if format is incorrect
            }

            string unit = parts[1].ToLower();
            DateTime scheduledDate = DateTime.UtcNow;

            if (unit.Contains("month"))
            {
                scheduledDate = scheduledDate.AddMonths(number);
            }
            else if (unit.Contains("year"))
            {
                scheduledDate = scheduledDate.AddYears(number);
            }
            else if (unit.Contains("day"))
            {
                scheduledDate = scheduledDate.AddDays(number);
            }

            return scheduledDate;
        }

    }
}
