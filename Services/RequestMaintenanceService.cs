using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sp_backend_March4.Interfaces;
using sp_backend_March4.Models;
using WeatherApi;

namespace sp_backend_March4.Services
{
    public class RequestMaintenanceService : IRequestMaintenanceService
    {
        private readonly AppDbContext _context;

        public RequestMaintenanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RequestMaintenance> CreateRequestMaintenanceAsync(RequestMaintenance request)
        {
            _context.RequestMaintenances.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<RequestMaintenance?> GetRequestMaintenanceByIdAsync(int id)
        {
            return await _context.RequestMaintenances
                .FirstOrDefaultAsync(rm => rm.Id == id);
        }

        public async Task<IEnumerable<RequestMaintenance>> GetAllRequestMaintenancesAsync()
        {
            return await _context.RequestMaintenances.ToListAsync();
        }

        public async Task<RequestMaintenance> UpdateRequestMaintenanceStatusAsync(int id, string status)
        {
            var request = await _context.RequestMaintenances
                .FirstOrDefaultAsync(rm => rm.Id == id);

            if (request == null)
            {
                throw new KeyNotFoundException($"RequestMaintenance with ID {id} not found.");
            }

            request.Status = status;

            await _context.SaveChangesAsync();
            return request;
        }
    }
}
