using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sp_backend.Interfaces;
using sp_backend_March4.Interfaces;
using sp_backend_March4.Models;
using WeatherApi;

namespace sp_backend_March4.Services
{
    public class RequestMaintenanceService : IRequestMaintenanceService
    {
        private readonly AppDbContext _context;
        private readonly IMissionService _missionService;


        public RequestMaintenanceService(AppDbContext context, IMissionService missionService)
        {
            _context = context;
            _missionService = missionService;
        }

        public async Task<RequestMaintenance> CreateRequestMaintenanceAsync(RequestMaintenance request)
        {
            _context.RequestMaintenances.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<RequestMaintenance?> GetRequestMaintenanceByIdAsync(int id)
        {
            var requestMaintenance = await _context.RequestMaintenances
                .Include(rm => rm.Maintenance)
                    .ThenInclude(m => m.SubEquipment)
                        .ThenInclude(se => se.Equipment)
                .FirstOrDefaultAsync(rm => rm.Id == id);

            if (requestMaintenance?.Maintenance?.SubEquipment?.EquipmentId is int equipmentId)
            {
                var maintenanceStart = requestMaintenance.Maintenance.MaintenanceDate;
                var maintenanceEnd = requestMaintenance.Maintenance.MaintenanceEndDate;

                var hasOverlap = await _context.Nonavailabilities
                    .AnyAsync(na =>
                        na.EquipmentId == equipmentId &&
                        na.Date1 <= maintenanceEnd &&
                        na.Date2 >= maintenanceStart
                    );

                if (hasOverlap)
                {
                    requestMaintenance.Details = "equipment in use";
                    await _context.SaveChangesAsync();
                }
                else
                {
                    requestMaintenance.Details = "null";
                    await _context.SaveChangesAsync();
                }
            }

            return requestMaintenance;
        }



        public async Task<IEnumerable<RequestMaintenance>> GetAllRequestMaintenancesAsync()
        {
            return await _context.RequestMaintenances.ToListAsync();
        }


        public async Task<RequestMaintenance> UpdateRequestMaintenanceStatusAsync(int id, string status)
        {
            var request = await _context.RequestMaintenances
                .Include(rm => rm.Maintenance)
                .FirstOrDefaultAsync(rm => rm.Id == id);

            if (request == null)
            {
                throw new KeyNotFoundException($"RequestMaintenance with ID {id} not found.");
            }

            request.Status = status;

            if (status == "Accepted" && request.Maintenance != null)
            {
                int equipmentId = (int)request.EquipmentId;
                var maintenanceStart = request.Maintenance.MaintenanceDate;
                var maintenanceEnd = request.Maintenance.MaintenanceEndDate;

                // Debug log the values
                Console.WriteLine($"Looking for overlapping missions for equipment {equipmentId}, from {maintenanceStart} to {maintenanceEnd}");

                // Get overlapping missions
                var overlappingMissions = await _context.EquipmentMissions
    .Where(em => em.EquipmentId == equipmentId)
    .Join(
        _context.Missions,
        em => em.MissionId,
        m => m.Id,
        (em, m) => m
    )
    .Where(m =>
        (m.StartTime < maintenanceEnd && m.StartTime >= maintenanceStart) ||
        (m.EndTime <= maintenanceEnd && m.EndTime >= maintenanceStart) ||
        (m.StartTime <= maintenanceStart && m.EndTime >= maintenanceEnd) ||
        (m.StartTime >= maintenanceStart && m.EndTime <= maintenanceEnd)
    )
    .ToListAsync();


                // Debugging step: Log the number of overlapping missions
                Console.WriteLine($"Found {overlappingMissions.Count} overlapping missions.");

                if (overlappingMissions.Count > 0)
                {
                    foreach (var mission in overlappingMissions)
                    {
                        // Debugging step: Log each mission found
                        Console.WriteLine($"Mission {mission.Id} overlaps with maintenance period for Equipment {equipmentId}");

                        var equipmentMission = await _context.EquipmentMissions
                            .FirstOrDefaultAsync(em => em.MissionId == mission.Id && em.EquipmentId == equipmentId);

                        if (equipmentMission != null)
                        {
                            _context.EquipmentMissions.Remove(equipmentMission);  // Remove the relationship
                            Console.WriteLine($"Removed EquipmentMission between Mission {mission.Id} and Equipment {equipmentId}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No overlapping missions found.");
                }

                // Handle nonavailability
                var overlappingNonavailabilities = await _context.Nonavailabilities
                    .Where(na =>
                        na.EquipmentId == equipmentId &&
                        (
                            (na.Date1 <= maintenanceEnd && na.Date1 >= maintenanceStart) ||
                            (na.Date2 <= maintenanceEnd && na.Date2 >= maintenanceStart) ||
                            (na.Date1 <= maintenanceStart && na.Date2 >= maintenanceEnd) ||
                            (na.Date1 >= maintenanceStart && na.Date2 <= maintenanceEnd)
                        )
                    )
                    .ToListAsync();

                _context.Nonavailabilities.RemoveRange(overlappingNonavailabilities);

                var newNonavailability = new Nonavailability
                {
                    EquipmentId = equipmentId,
                    Date1 = maintenanceStart,
                    Date2 = maintenanceEnd
                };

                _context.Nonavailabilities.Add(newNonavailability);
            }

            await _context.SaveChangesAsync();
            return request;
        }




        public async Task<object> GetRequestMaintenancesByEquipmentIdAsync(int equipmentId)
        {
            var requests = await _context.RequestMaintenances
                .Where(r => r.EquipmentId == equipmentId)
                .ToListAsync();

            var responseList = new List<object>();
            bool isEquipmentInUse = false;

            foreach (var request in requests)
            {
                var result = new Dictionary<string, object>
            {
                { "id", request.Id },
                { "status", request.Status },
                { "details", request.Details },
                { "cycle", request.Cycle },
                { "equipmentId", request.EquipmentId },
            };

                if (!string.IsNullOrEmpty(request.Details) &&
                    request.Details.ToLower().Contains("equipment in use"))
                {
                    result.Add("equipmentInUseInMission", true);
                    isEquipmentInUse = true;
                }

                responseList.Add(result);
            }

            return new
            {
                requests = responseList,
                message = isEquipmentInUse ? "equipment in use" : null
            };
        }

    }
}
