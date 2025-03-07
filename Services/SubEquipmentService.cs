using WeatherApi.Interfaces;
using WeatherApi.Models;
using Microsoft.EntityFrameworkCore;
using WeatherApi.DTOs;
using sp_backend.DTO;
using sp_backend.Models;

namespace WeatherApi.Services
{
    public class SubEquipmentService : ISubEquipmentService
    {
        private readonly AppDbContext _context;

        public SubEquipmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubEquipmentDto>> GetAllSubEquipmentsAsync()
        {
            var subEquipments = await _context.SubEquipments
                .Include(s => s.Equipment)
                .Include(s => s.Maintenances)
                .ToListAsync();

            return subEquipments.Select(s => new SubEquipmentDto
            {
                Id = s.Id,
                Name = s.Name,
                Cycle = s.Cycle,
                Status = s.Status,
                CreationDate = s.CreationDate,
                EquipmentId = s.Equipment?.Id,
                Maintenances = s.Maintenances.Select(m => new MaintenanceDTO
                {
                    Id = m.Id,
                    Description = m.Description,
                    Type = m.Type,
                    Name = m.Name,
                    SubEquipmentId = m.SubEquipmentId,
                    Items = m.Items
                }).ToList()
            }).ToList();
        }

        public async Task<SubEquipmentDto?> GetSubEquipmentByIdAsync(int id)
        {
            var subEquipment = await _context.SubEquipments
                .Include(s => s.Equipment)
                .Include(s => s.Maintenances)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (subEquipment == null) return null;

            return new SubEquipmentDto
            {
                Id = subEquipment.Id,
                Name = subEquipment.Name,
                Cycle = subEquipment.Cycle,
                Status = subEquipment.Status,
                CreationDate = subEquipment.CreationDate,
                EquipmentId = subEquipment.Equipment?.Id,
                Maintenances = subEquipment.Maintenances.Select(m => new MaintenanceDTO
                {
                    Id = m.Id,
                    Description = m.Description,
                    Type = m.Type,
                    Name = m.Name,
                    SubEquipmentId = m.SubEquipmentId
                }).ToList()
            };
        }



        public async Task<SubEquipment> CreateSubEquipmentAsync(SubEquipment subEquipment)
        {
            _context.SubEquipments.Add(subEquipment);
            await _context.SaveChangesAsync(); // Save first to get a valid ID

            // Compute Maintenance Date
            DateTime scheduledDate = ComputeMaintenanceDate(subEquipment.Cycle);

            // Create Maintenance
            var maintenance = new Maintenance
            {
                Description = $"Initial maintenance for {subEquipment.Name}",
                MaintenanceDate = scheduledDate,
                SubEquipmentId = subEquipment.Id
            };

            _context.Maintenances.Add(maintenance); // Attach maintenance to subEquipment

            // Find the related Equipment
            var equipment = await _context.Equipments.FindAsync(subEquipment.EquipmentId);
            if (equipment != null && subEquipment.Status != "bon_etat")
            {
                // If the new SubEquipment is not in "bon_etat", mark Equipment as unavailable
                equipment.Availability = false;
            }

            await _context.SaveChangesAsync(); // Save all changes

            return subEquipment;
        }



        public async Task<SubEquipment?> UpdateSubEquipmentAsync(int id, SubEquipment subEquipment)
        {
            var existingSubEquipment = await _context.SubEquipments.FindAsync(id);
            if (existingSubEquipment == null)
            {
                return null;
            }

            // Check if the status is changing
            bool wasBroken = existingSubEquipment.Status == "en_panne";
            bool isNowNormal = subEquipment.Status == "bon_etat";
            bool wasWorking = existingSubEquipment.Status != "En panne";
            bool isNowBroken = subEquipment.Status == "en_panne";

            existingSubEquipment.Name = subEquipment.Name ?? existingSubEquipment.Name;
            existingSubEquipment.Cycle = subEquipment.Cycle ?? existingSubEquipment.Cycle;
            existingSubEquipment.Status = subEquipment.Status ?? existingSubEquipment.Status;

            // Save changes immediately to reflect the updated status in the database
            await _context.SaveChangesAsync();

            // Find the related Equipment
            var equipment = await _context.Equipments.FindAsync(existingSubEquipment.EquipmentId);
            if (equipment != null)
            {
                if (wasWorking && isNowBroken)
                {
                    // If a SubEquipment breaks down, mark the Equipment as unavailable
                    equipment.Availability = false;
                }
                else if (wasBroken && isNowNormal)
                {
                    // Check if all other SubEquipments are "Normal" AND include this one as "Normal"
                    bool allNormal = await _context.SubEquipments
                        .Where(se => se.EquipmentId == existingSubEquipment.EquipmentId && se.Id != existingSubEquipment.Id)
                        .AllAsync(se => se.Status == "bon_etat");

                    if (allNormal && isNowNormal) // Ensure the current one is also "Normal"
                    {
                        equipment.Availability = true;
                    }
                }

                await _context.SaveChangesAsync(); // Save Equipment changes
            }

            return existingSubEquipment;
        }





        public async Task<bool> DeleteSubEquipmentAsync(int id)
        {
            var subEquipment = await _context.SubEquipments.FindAsync(id);
            if (subEquipment == null)
            {
                return false;
            }

            _context.SubEquipments.Remove(subEquipment);
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
