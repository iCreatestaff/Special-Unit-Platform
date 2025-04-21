using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sp_backend.Interfaces;
using sp_backend.Models;
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



        public async Task<IEnumerable<object>> GetAllRequestMaintenancesAsync()
        {
            var result = await _context.RequestMaintenances
                .Select(r => r.EquipmentId)
                .Distinct()
                .Join(_context.Equipments,
                      equipmentId => equipmentId,
                      equipment => equipment.Id,
                      (equipmentId, equipment) => new
                      {
                          equipmentId = equipment.Id,
                          equipmentName = equipment.Name
                      })
                .ToListAsync();

            return result;
        }



        public async Task<RequestMaintenance> UpdateRequestMaintenanceStatusAsync(int id, string status)
        {
            var request = await _context.RequestMaintenances
                .Include(rm => rm.Maintenance)
                .ThenInclude(m => m.SubEquipment)
                .FirstOrDefaultAsync(rm => rm.Id == id);

            if (request == null)
                throw new KeyNotFoundException($"RequestMaintenance with ID {id} not found.");

            request.Status = status;

            if (status == "Accepted" && request.Maintenance != null)
            {
                int equipmentId = (int)request.EquipmentId;
                var maintenanceStart = request.Maintenance.MaintenanceDate;
                var maintenanceEnd = request.Maintenance.MaintenanceEndDate;

                // Log overlapping mission check
                Console.WriteLine($"Looking for overlapping missions for equipment {equipmentId}, from {maintenanceStart} to {maintenanceEnd}");

                var overlappingMissions = await _context.EquipmentMissions
                    .Where(em => em.EquipmentId == equipmentId)
                    .Join(
                        _context.Missions,
                        em => em.MissionId,
                        m => m.Id,
                        (em, m) => m
                    )
                    .Where(m =>
                        (m.StartTime < maintenanceEnd && m.EndTime > maintenanceStart)
                    )
                    .ToListAsync();

                Console.WriteLine($"Found {overlappingMissions.Count} overlapping missions.");

                foreach (var mission in overlappingMissions)
                {
                    Console.WriteLine($"Mission {mission.Id} overlaps with maintenance period for Equipment {equipmentId}");

                    var equipmentMission = await _context.EquipmentMissions
                        .FirstOrDefaultAsync(em => em.MissionId == mission.Id && em.EquipmentId == equipmentId);

                    if (equipmentMission != null)
                    {
                        _context.EquipmentMissions.Remove(equipmentMission);
                        Console.WriteLine($"Removed EquipmentMission between Mission {mission.Id} and Equipment {equipmentId}");
                    }
                }

                // Handle overlapping nonavailabilities
                var overlappingNonavailabilities = await _context.Nonavailabilities
                    .Where(na =>
                        na.EquipmentId == equipmentId &&
                        (
                            (na.Date1 < maintenanceEnd && na.Date2 > maintenanceStart)
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

                // Create next cycle Maintenance and RequestMaintenance
                var subEquipment = request.Maintenance.SubEquipment;
                var nextMaintenanceDate = ComputeMaintenanceDate(request.Cycle, request.Maintenance.MaintenanceDate);
                request.Maintenance.Status = "Scheduled";

                var nextMaintenance = new Maintenance
                {
                    Name = $"Scheduled maintenance for {subEquipment.Name}",
                    Description = $"Next maintenance cycle for {subEquipment.Name}",
                    MaintenanceDate = nextMaintenanceDate,
                    MaintenanceEndDate = nextMaintenanceDate + TimeSpan.FromHours(1),
                    SubEquipmentId = subEquipment.Id,
                    Cycle = subEquipment.Cycle
                };

                _context.Maintenances.Add(nextMaintenance);

                var nextRequest = new RequestMaintenance
                {
                    Status = "Pending",
                    Details = $"Automatically generated request for next maintenance of {subEquipment.Name}",
                    Cycle = subEquipment.Cycle,
                    EquipmentId = subEquipment.EquipmentId,
                    Maintenance = nextMaintenance
                };

                _context.RequestMaintenances.Add(nextRequest);
            }

            else if (status == "Rejected" && request.Maintenance != null)
            {
                int equipmentId = (int)request.EquipmentId;

                // Find the latest mission using this equipment
                var lastMission = await _context.EquipmentMissions
                    .Where(em => em.EquipmentId == equipmentId)
                    .Join(
                        _context.Missions,
                        em => em.MissionId,
                        m => m.Id,
                        (em, m) => m
                    )
                    .Where(m => m.EndTime > DateTime.UtcNow)
                    .OrderByDescending(m => m.EndTime)
                    .FirstOrDefaultAsync();

                if (lastMission != null)
                {
                    DateTime nextMaintenanceStart = lastMission.EndTime;


                    // Create a new Maintenance
                    var newMaintenance = new Maintenance
                    {
                        Name = request.Maintenance.SubEquipment?.Name ?? "Unnamed",
                        Description = $"Delayed maintenance for {request.Maintenance.SubEquipment?.Name ?? "equipment"}",
                        MaintenanceDate = nextMaintenanceStart,
                        MaintenanceEndDate = nextMaintenanceStart + TimeSpan.FromHours(1),
                        SubEquipmentId = request.Maintenance.SubEquipmentId,
                        Cycle = request.Cycle
                    };
                    _context.Maintenances.Add(newMaintenance);

                    // Create a new RequestMaintenance
                    var newRequest = new RequestMaintenance
                    {
                        Status = "Pending",
                        Details = $"Rescheduled maintenance for {request.Maintenance.SubEquipment?.Name ?? "equipment"} after rejection",
                        Cycle = request.Cycle,
                        EquipmentId = equipmentId,
                        Maintenance = newMaintenance
                    };
                    _context.RequestMaintenances.Add(newRequest);
                }

                // Remove the old rejected maintenance
                _context.Maintenances.Remove(request.Maintenance);
            }


            await _context.SaveChangesAsync();
            return request;
        }





        public async Task<object> GetRequestMaintenancesByEquipmentIdAsync(int equipmentId)
        {
            var requests = await _context.RequestMaintenances
                .Where(r => r.EquipmentId == equipmentId)
                .Include(rm => rm.Maintenance)
                    .ThenInclude(m => m.SubEquipment)
                        .ThenInclude(se => se.Equipment)
                .ToListAsync();

            var responseList = new List<object>();
            bool isEquipmentInUse = false;

            // Fetch the equipment name using the equipmentId
            var equipment = await _context.Equipments.FindAsync(equipmentId);
            string? equipmentName = equipment?.Name;

            foreach (var request in requests)
            {
                bool equipmentInUseInMission = false;

                if (request?.Maintenance?.SubEquipment?.EquipmentId is int eqId)
                {
                    var maintenanceStart = request.Maintenance.MaintenanceDate;
                    var maintenanceEnd = request.Maintenance.MaintenanceEndDate;

                    var hasOverlap = await _context.Nonavailabilities
                        .AnyAsync(na =>
                            na.EquipmentId == eqId &&
                            na.Date1 <= maintenanceEnd &&
                            na.Date2 >= maintenanceStart
                        );

                    if (hasOverlap)
                    {
                        equipmentInUseInMission = true;
                        request.Details = "equipment in use";
                    }
                    else
                    {
                        request.Details = "null";
                    }

                    await _context.SaveChangesAsync(); // Update request details
                }

                var result = new Dictionary<string, object>
        {
            { "id", request.Id },
            { "status", request.Status },
            { "details", request.Details },
            { "cycle", request.Cycle },
            { "equipmentId", request.EquipmentId },
            { "equipmentInUseInMission", equipmentInUseInMission }
        };

                if (equipmentInUseInMission)
                {
                    isEquipmentInUse = true;
                }

                responseList.Add(result);
            }

            return new
            {
                equipmentName = equipmentName,
                requests = responseList,
                message = isEquipmentInUse ? "equipment in use" : null
            };
        }





        private DateTime ComputeMaintenanceDate(string? cycle, DateTime? baseDate = null)
        {
            if (string.IsNullOrWhiteSpace(cycle))
            {
                return DateTime.UtcNow; // Default to today if cycle is invalid
            }

            var parts = cycle.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2 || !int.TryParse(parts[0], out int number))
            {
                return DateTime.UtcNow; // Default if format is incorrect
            }

            string unit = parts[1].ToLower();
            DateTime scheduledDate = baseDate ?? DateTime.UtcNow;

            if (unit.StartsWith("month"))
            {
                scheduledDate = scheduledDate.AddMonths(number);
            }
            else if (unit.StartsWith("year"))
            {
                scheduledDate = scheduledDate.AddYears(number);
            }
            else if (unit.StartsWith("day"))
            {
                scheduledDate = scheduledDate.AddDays(number);
            }
            else if (unit.StartsWith("hour"))
            {
                scheduledDate = scheduledDate.AddHours(number);
            }
            else if (unit.StartsWith("minute"))
            {
                scheduledDate = scheduledDate.AddMinutes(number);
            }
            else if (unit.StartsWith("second"))
            {
                scheduledDate = scheduledDate.AddSeconds(number);
            }

            return scheduledDate;
        }


    }
}
