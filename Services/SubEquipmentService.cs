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

            existingSubEquipment.Name = subEquipment.Name ?? existingSubEquipment.Name;
            existingSubEquipment.Cycle = subEquipment.Cycle ?? existingSubEquipment.Cycle;
            existingSubEquipment.Status = subEquipment.Status ?? existingSubEquipment.Status;

            await _context.SaveChangesAsync();
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
