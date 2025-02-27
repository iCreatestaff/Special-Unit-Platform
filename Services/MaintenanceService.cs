using WeatherApi.Interfaces;
using sp_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace WeatherApi.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly AppDbContext _context;

        public MaintenanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Maintenance>> GetAllMaintenancesAsync()
        {
            return await _context.Maintenances
                .Include(m => m.SubEquipment) // Include SubEquipment
                .Include(m => m.Items) // Include Items
                .ToListAsync();
        }

        public async Task<Maintenance?> GetMaintenanceByIdAsync(int id)
        {
            return await _context.Maintenances
                .Include(m => m.SubEquipment) // Include SubEquipment
                .Include(m => m.Items) // Include Items
                .FirstOrDefaultAsync(m => m.Id == id);
        }


        public async Task<Maintenance> CreateMaintenanceAsync(Maintenance maintenance)
        {
            if (maintenance.SubEquipmentId.HasValue)
            {
                var subEquipmentExists = await _context.SubEquipments
                    .AnyAsync(se => se.Id == maintenance.SubEquipmentId.Value);

                if (!subEquipmentExists)
                {
                    throw new ArgumentException($"SubEquipment with ID {maintenance.SubEquipmentId.Value} does not exist.");
                }
            }

            _context.Maintenances.Add(maintenance);
            await _context.SaveChangesAsync();
            return maintenance;
        }



        public async Task<Maintenance?> UpdateMaintenanceAsync(int id, Maintenance maintenance)
        {
            var existingMaintenance = await _context.Maintenances.FindAsync(id);
            if (existingMaintenance == null)
            {
                return null;
            }

            existingMaintenance.Name = maintenance.Name ?? existingMaintenance.Name;
            existingMaintenance.Type = maintenance.Type ?? existingMaintenance.Type;
            existingMaintenance.Description = maintenance.Description ?? existingMaintenance.Description;
            existingMaintenance.MaintenanceDate = maintenance.MaintenanceDate;
            existingMaintenance.SubEquipmentId = maintenance.SubEquipmentId ?? existingMaintenance.SubEquipmentId;

            await _context.SaveChangesAsync();
            return existingMaintenance;
        }

        public async Task<bool> DeleteMaintenanceAsync(int id)
        {
            var maintenance = await _context.Maintenances.FindAsync(id);
            if (maintenance == null)
            {
                return false;
            }

            _context.Maintenances.Remove(maintenance);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
