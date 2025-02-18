using WeatherApi.Interfaces;
using WeatherApi.Models;
using Microsoft.EntityFrameworkCore;

namespace WeatherApi.Services
{
    public class SubEquipmentService : ISubEquipmentService
    {
        private readonly AppDbContext _context;

        public SubEquipmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SubEquipment>> GetAllSubEquipmentsAsync()
        {
            return await _context.SubEquipments.Include(s => s.Equipment).ToListAsync();
        }

        public async Task<SubEquipment?> GetSubEquipmentByIdAsync(int id)
        {
            return await _context.SubEquipments.Include(s => s.Equipment)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<SubEquipment> CreateSubEquipmentAsync(SubEquipment subEquipment)
        {
            _context.SubEquipments.Add(subEquipment);
            await _context.SaveChangesAsync();
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
            bool wasBroken = existingSubEquipment.Status == "broken-down";
            bool isNowNormal = subEquipment.Status == "Normal";
            bool wasWorking = existingSubEquipment.Status != "broken-down";
            bool isNowBroken = subEquipment.Status == "broken-down";

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
                        .AllAsync(se => se.Status == "Normal");

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
    }
}
