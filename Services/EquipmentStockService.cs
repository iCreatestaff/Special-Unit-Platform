using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using sp_backend.DTO;
using sp_backend.Models;
using sp_backend_March4.Models;
using WeatherApi;
using WeatherApi.Models;

namespace sp_backend.Services
{
    public class EquipmentStockService : IEquipmentStockService
    {
        private readonly WeatherApi.AppDbContext _context;
        private readonly IMapper _mapper;

        public EquipmentStockService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<EquipmentStockDTO>> GetAllAsync()
        {
            return await _context.EquipmentStocks
                .Include(es => es.Equipments)
                    .ThenInclude(e => e.SubEquipments)
                .AsSplitQuery() // Enables query splitting
                .ProjectTo<EquipmentStockDTO>(_mapper.ConfigurationProvider)
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
                    Photo = es.Photo,
                    Equipments = es.Equipments.Select(e => new Equipment
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Type = e.Type,
                        Availability = e.Availability,
                        Photo = e.Photo,
                        SubEquipments = e.SubEquipments != null ? e.SubEquipments.Select(se => new SubEquipment
                        {
                            Id = se.Id,
                            Name = se.Name,
                            Cycle = se.Cycle,
                            Status = se.Status,
                            CreationDate = se.CreationDate,
                            EquipmentId = se.EquipmentId,
                        }).ToList() : new List<SubEquipment>()
                    }).ToList()

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
            // Don't include Equipments to ensure no modification

            if (existing == null) return false;

            // Update only if a value is provided
            if (!string.IsNullOrEmpty(equipmentStockDto.EquipmentName))
            {
                existing.EquipmentName = equipmentStockDto.EquipmentName;
            }

            if (equipmentStockDto.Quantity != default) // Ensure Quantity isn't unintentionally set to 0
            {
                existing.Quantity = equipmentStockDto.Quantity;
            }

            if (!string.IsNullOrEmpty(equipmentStockDto.Photo))
            {
                existing.Photo = equipmentStockDto.Photo;
            }

            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<Equipment>> UpdateEquipmentsByEquipmentStockIdAsync(int equipmentStockId, Equipment updatedEquipment)
        {
            // Find the EquipmentStock by its ID
            var equipmentStock = await _context.EquipmentStocks
                .Include(es => es.Equipments)
                    .ThenInclude(e => e.SubEquipments)
                .AsSplitQuery()
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

                if (updatedEquipment.Availability.HasValue)
                {
                    equipment.Availability = updatedEquipment.Availability;
                }

                if (!string.IsNullOrEmpty(updatedEquipment.Name))
                {
                    equipment.Name = updatedEquipment.Name;
                    equipmentStock.EquipmentName = updatedEquipment.Name;
                }

                if (!string.IsNullOrEmpty(updatedEquipment.Photo))
                {
                    equipment.Photo = updatedEquipment.Photo;
                }

                // Handle SubEquipments update (by name matching, but NOT updating Status)
                if (updatedEquipment.SubEquipments != null)
                {
                    var existingSubEquipments = equipment.SubEquipments.ToDictionary(se => se.Name, StringComparer.OrdinalIgnoreCase);

                    foreach (var subEquipment in updatedEquipment.SubEquipments)
                    {
                        if (existingSubEquipments.TryGetValue(subEquipment.Name, out var existingSubEquipment))
                        {
                            // ✅ Check if cycle has changed
                            if (existingSubEquipment.Cycle != subEquipment.Cycle)
                            {
                                existingSubEquipment.Cycle = subEquipment.Cycle;

                                // 🔹 Update Maintenance Date
                                var maintenance = await _context.Maintenances
                                    .FirstOrDefaultAsync(m => m.SubEquipmentId == existingSubEquipment.Id && m.MaintenanceDate > DateTime.Now);

                                if (maintenance != null)
                                {
                                    maintenance.Cycle = existingSubEquipment.Cycle;
                                    // maintenance.MaintenanceDate = ComputeMaintenanceDate(existingSubEquipment.Cycle);
                                    // maintenance.MaintenanceEndDate = ComputeMaintenanceDate(existingSubEquipment.Cycle) + TimeSpan.FromHours(1);

                                    if (maintenance.RequestMaintenance != null)
                                    {
                                        maintenance.RequestMaintenance.Cycle = existingSubEquipment.Cycle;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Add new subequipment if name does not exist
                            var newSubEquipment = new SubEquipment
                            {
                                Name = subEquipment.Name,
                                Cycle = subEquipment.Cycle,
                                Status = "bon_etat", // Keep provided status for new subequipments
                                CreationDate = DateTime.UtcNow,
                                EquipmentId = equipment.Id
                            };

                            _context.SubEquipments.Add(newSubEquipment);
                            await _context.SaveChangesAsync(); // Save to get ID

                            // Create maintenance record for new subequipment
                            var maintenance = new Maintenance
                            {
                                Name = subEquipment.Name,
                                Description = $"Initial maintenance for {newSubEquipment.Name} of equipment {newSubEquipment.EquipmentId}",
                                MaintenanceDate = ComputeMaintenanceDate(newSubEquipment.Cycle) + TimeSpan.FromHours(1.5) + TimeSpan.FromMinutes(2),
                                MaintenanceEndDate = ComputeMaintenanceDate(newSubEquipment.Cycle) + TimeSpan.FromHours(2),
                                SubEquipmentId = newSubEquipment.Id,
                                Cycle = subEquipment.Cycle,
                                Status = "Pending"
                            };

                            _context.Maintenances.Add(maintenance);

                            var requestmaintenance = new RequestMaintenance
                            {
                                Status = "Pending",
                                Details = $"Initial Request for {newSubEquipment.Name}",
                                Cycle = newSubEquipment.Cycle,
                                EquipmentId = newSubEquipment.EquipmentId,
                                Maintenance = maintenance
                            };
                            _context.RequestMaintenances.Add(requestmaintenance);
                        }
                    }
                }
            }

            // Save all changes
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

        public async Task<bool> AddSubEquipmentToAllEquipmentsAsync(int equipmentStockId, SubEquipment newSubEquipment)
        {
            // Fetch the EquipmentStock with related Equipments
            var equipmentStock = await _context.EquipmentStocks
                .Where(es => es.Id == equipmentStockId)
                .Include(es => es.Equipments)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if (equipmentStock == null || !equipmentStock.Equipments.Any())
            {
                return false; // No matching EquipmentStock or Equipments found
            }

            // Add the new SubEquipment to each Equipment under the EquipmentStock
            foreach (var equipment in equipmentStock.Equipments)
            {
                var subEquipment = new SubEquipment
                {
                    Name = newSubEquipment.Name,
                    Cycle = newSubEquipment.Cycle,
                    Status = newSubEquipment.Status, // Preserve provided status
                    CreationDate = DateTime.UtcNow,
                    EquipmentId = equipment.Id
                };

                _context.SubEquipments.Add(subEquipment);
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteSubEquipmentFromAllEquipmentsAsync(int equipmentStockId, string subEquipmentName)
        {
            var equipmentStock = await _context.EquipmentStocks
                .Where(es => es.Id == equipmentStockId)
                .Include(es => es.Equipments)
                    .ThenInclude(e => e.SubEquipments)
                .AsSplitQuery()
                .FirstOrDefaultAsync();

            if (equipmentStock == null || !equipmentStock.Equipments.Any())
            {
                return false;
            }

            var subEquipmentsToRemove = equipmentStock.Equipments
                .SelectMany(e => e.SubEquipments)
                .Where(se => se.Name == subEquipmentName)
                .ToList();

            if (!subEquipmentsToRemove.Any())
            {
                return false;
            }

            _context.SubEquipments.RemoveRange(subEquipmentsToRemove);
            await _context.SaveChangesAsync();

            return true;
        }




        public async Task<bool> DeleteAsync(int id)
        {
            var equipmentStock = await _context.EquipmentStocks.FindAsync(id);
            if (equipmentStock == null) return false;

            _context.EquipmentStocks.Remove(equipmentStock);
            return await _context.SaveChangesAsync() > 0;
        }



        private DateTime ComputeMaintenanceDate(string? cycle)
        {
            if (string.IsNullOrWhiteSpace(cycle))
            {
                return DateTime.UtcNow; // Default to today if cycle is invalid
            }

            var parts = cycle.Split(' ');
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
