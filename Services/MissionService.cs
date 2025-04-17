using sp_backend.DTO;
using sp_backend.Interfaces;
using sp_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi;
using WeatherApi.Models;

public class MissionService : IMissionService
{
    private readonly AppDbContext _context;

    public MissionService(AppDbContext context)
    {
        _context = context;
    }

    // Create a new mission and associate it with the admin and assigned accounts/equipment
    public async Task<bool> CreateMissionAsync(MissionDTO missionDTO)
    {
        try
        {
            // Validate AdminId
            var adminAccount = await _context.Accounts.FindAsync(missionDTO.AdminId);
            if (adminAccount == null || adminAccount.Role != "Admin")
            {
                return false; // Admin validation failed
            }

            var mission = new Mission
            {
                Description = missionDTO.Description,
                Type = missionDTO.Type,
                StartTime = missionDTO.StartTime,
                EndTime = missionDTO.EndTime,
                Location = missionDTO.Location,
                AdminId = missionDTO.AdminId
            };

            _context.Missions.Add(mission);
            await _context.SaveChangesAsync(); // Save first to get Mission ID

            var nonavailabilities = new List<Nonavailability>();

            // Add assigned accounts
            if (missionDTO.AssignedAccounts != null && missionDTO.AssignedAccounts.Any())
            {
                var validAccounts = await _context.Accounts
                    .Where(a => missionDTO.AssignedAccounts.Contains(a.Id))
                    .Select(a => a.Id)
                    .ToListAsync();

                var accountMissions = validAccounts
                    .Select(accountId => new AccountMission { MissionId = mission.Id, AccountId = accountId })
                    .ToList();

                _context.AccountMissions.AddRange(accountMissions);

                // Create nonavailability records for assigned accounts
                nonavailabilities.AddRange(validAccounts.Select(accountId => new Nonavailability
                {
                    Date1 = mission.StartTime,
                    Date2 = mission.EndTime,
                    Type = "Account",
                    AccountId = accountId,
                    MissionID = mission.Id,
                    Reason = "Mission"
                }));
            }

            // Add assigned equipment
            if (missionDTO.AssignedEquipments != null && missionDTO.AssignedEquipments.Any())
            {
                var validEquipments = await _context.Equipments
                    .Where(e => missionDTO.AssignedEquipments.Contains(e.Id))
                    .Select(e => e.Id)
                    .ToListAsync();

                var equipmentMissions = validEquipments
                    .Select(equipmentId => new EquipmentMission { MissionId = mission.Id, EquipmentId = equipmentId })
                    .ToList();

                _context.EquipmentMissions.AddRange(equipmentMissions);

                // Create nonavailability records for assigned equipment
                nonavailabilities.AddRange(validEquipments.Select(equipmentId => new Nonavailability
                {
                    Date1 = mission.StartTime,
                    Date2 = mission.EndTime,
                    Type = "Equipment",
                    EquipmentId = equipmentId,
                    MissionID = mission.Id,
                    Reason = $"Mission {mission.Id}"

                }));
            }

            // Add nonavailability records to DB
            if (nonavailabilities.Any())
            {
                _context.Nonavailabilities.AddRange(nonavailabilities);
            }

            // Save related assignments and nonavailability records
            int changes = await _context.SaveChangesAsync();
            if (changes == 0)
            {
                Console.WriteLine("Error: No database changes detected");
            }

            return changes > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating mission: {ex.Message}");
            return false;
        }
    }

    public async Task<List<MissionDTO>> GetMissionsByTypeAsync(string type)
    {
        if (string.IsNullOrEmpty(type))
        {
            throw new ArgumentException("Mission type is required.", nameof(type));
        }

        var missions = await _context.Missions
            .Where(m => m.Type == type)
            .Select(m => new MissionDTO
            {
                Id = m.Id,
                Type = m.Type,
                Description = m.Description,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Status = m.Status
            })
            .ToListAsync();

        return missions;
    }

    public async Task<List<MissionDTO>> GetMissionsByAgentIdAsync(int agentId)
    {
        var missions = await _context.Missions
            .Where(m => m.AccountMissions.Any(am => am.AccountId == agentId))
            .Include(m => m.AccountMissions)
            .Include(m => m.EquipmentMissions)
            .AsSplitQuery()  // This tells EF to execute separate queries for the related collections
            .ToListAsync();

        var missionDTOs = missions.Select(m => new MissionDTO
        {
            Id = m.Id,
            Type = m.Type,
            Description = m.Description,
            StartTime = m.StartTime,
            EndTime = m.EndTime,
            Location = m.Location,
            Status = m.Status,
            AssignedAccounts = m.AccountMissions.Select(am => am.AccountId).ToList(),
            AssignedEquipments = m.EquipmentMissions.Select(em => em.EquipmentId).ToList(),
            AdminId = m.AdminId
        }).ToList();

        return missionDTOs;
    }






    // Get mission by ID, including related accounts and equipment
    public async Task<MissionDTO?> GetMissionByIdAsync(int id)
    {
        var mission = await _context.Missions
            .Include(m => m.AccountMissions) // Load AccountMission relations
            .Include(m => m.EquipmentMissions)
            .AsSplitQuery() // Load EquipmentMission relations
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mission == null) return null;

        return new MissionDTO
        {
            Id = mission.Id,
            Description = mission.Description,
            Type = mission.Type,
            StartTime = mission.StartTime,
            EndTime = mission.EndTime,
            Location = mission.Location,
            Status = mission.Status,
            AdminId = mission.AdminId,

            // Extract related Account IDs
            AssignedAccounts = mission.AccountMissions.Select(am => am.AccountId).ToList(),

            // Extract related Equipment IDs
            AssignedEquipments = mission.EquipmentMissions.Select(em => em.EquipmentId).ToList()
        };
    }

    public async Task<bool> IsEquipmentAssignedToMissionAsync(int missionId, int equipmentId)
    {
        var mission = await _context.Missions.FindAsync(missionId);

        if (mission == null || mission.AssignedEquipments == null)
            return false;

        return mission.AssignedEquipments.Contains(equipmentId);
    }



    // Get all missions, including related accounts and equipment
    public async Task<List<MissionDTO>> GetAllMissionsAsync()
    {
        return await _context.Missions
            .Include(m => m.AccountMissions) // Load AccountMission relations
            .Include(m => m.EquipmentMissions)
            .AsSplitQuery() // Load EquipmentMission relations
            .Select(m => new MissionDTO
            {
                Id = m.Id,
                Description = m.Description,
                Type = m.Type,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Location = m.Location,
                Status = m.Status,
                AdminId = m.AdminId,

                // Extract related Account IDs
                AssignedAccounts = m.AccountMissions.Select(am => am.AccountId).ToList(),

                // Extract related Equipment IDs
                AssignedEquipments = m.EquipmentMissions.Select(em => em.EquipmentId).ToList()
            })
            .ToListAsync();
    }


    public async Task DeleteNonavailabilitiesByMissionId(int missionId)
    {
        var existingNonavailabilities = await _context.Nonavailabilities
            .Where(n => n.MissionID == missionId)
            .ToListAsync();

        if (existingNonavailabilities.Any())
        {
            _context.Nonavailabilities.RemoveRange(existingNonavailabilities);
            await _context.SaveChangesAsync();
        }
    }



    // Update an existing mission and associate it with updated accounts/equipment
    public async Task<bool> UpdateMissionAsync(int id, MissionDTO missionDTO)
    {
        var mission = await _context.Missions
            .Include(m => m.AccountMissions)
            .Include(m => m.EquipmentMissions)
            .AsSplitQuery()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mission == null) return false;

        // Update mission details
        mission.Description = missionDTO.Description;
        mission.Type = missionDTO.Type;
        mission.StartTime = missionDTO.StartTime;
        mission.EndTime = missionDTO.EndTime;
        mission.Location = missionDTO.Location;
        mission.Status = missionDTO.Status;
        mission.AdminId = missionDTO.AdminId;


        // Handle assigned accounts
        var existingAccountIds = mission.AccountMissions.Select(am => am.AccountId).ToList();
        var newAccountIds = missionDTO.AssignedAccounts ?? new List<int>();

        // Remove accounts that are no longer assigned
        mission.AccountMissions.RemoveAll(am => !newAccountIds.Contains(am.AccountId));

        // Add new assigned accounts and their nonavailability records
        foreach (var accountId in newAccountIds.Except(existingAccountIds))
        {
            mission.AccountMissions.Add(new AccountMission { MissionId = id, AccountId = accountId });
        }

        // Handle assigned equipment
        var existingEquipmentIds = mission.EquipmentMissions.Select(em => em.EquipmentId).ToList();
        var newEquipmentIds = missionDTO.AssignedEquipments ?? new List<int>();

        // Remove equipment that is no longer assigned
        mission.EquipmentMissions.RemoveAll(em => !newEquipmentIds.Contains(em.EquipmentId));

        // Add new assigned equipment and their nonavailability records
        foreach (var equipmentId in newEquipmentIds.Except(existingEquipmentIds))
        {
            mission.EquipmentMissions.Add(new EquipmentMission { MissionId = id, EquipmentId = equipmentId });
        }

        // Add new nonavailability records for assigned accounts and equipment
        var newNonavailabilities = new List<Nonavailability>();

        foreach (var accountId in newAccountIds)
        {
            newNonavailabilities.Add(new Nonavailability
            {
                Date1 = mission.StartTime,
                Date2 = mission.EndTime,
                Type = "Account",
                AccountId = accountId,
                MissionID = mission.Id
            });
        }

        foreach (var equipmentId in newEquipmentIds)
        {
            newNonavailabilities.Add(new Nonavailability
            {
                Date1 = mission.StartTime,
                Date2 = mission.EndTime,
                Type = "Equipment",
                EquipmentId = equipmentId,
                MissionID = mission.Id
            });
        }

        // Add new nonavailability records to the database
        await _context.Nonavailabilities.AddRangeAsync(newNonavailabilities);

        return await _context.SaveChangesAsync() > 0;
    }



    // Delete a mission by ID
    public async Task<bool> DeleteMissionAsync(int id)
    {
        // Delete related Nonavailabilities first
        var existingNonavailabilities = await _context.Nonavailabilities
            .Where(n => n.MissionID == id)
            .ToListAsync();

        if (existingNonavailabilities.Any())
        {
            _context.Nonavailabilities.RemoveRange(existingNonavailabilities);
        }

        // Find the mission
        var mission = await _context.Missions.FindAsync(id);
        if (mission == null) return false;

        // Remove the mission
        _context.Missions.Remove(mission);

        // Save changes
        return await _context.SaveChangesAsync() > 0;
    }


    public async Task<bool> RemoveEquipmentFromMissionAsync(int missionId, int equipmentId)
    {
        var mission = await _context.Missions
    .FirstOrDefaultAsync(m => m.Id == missionId);

        if (mission == null)
            throw new KeyNotFoundException($"Mission with ID {missionId} not found.");

        // Load the list of assigned equipment IDs manually if needed
        mission.AssignedEquipments = await _context.EquipmentMissions
            .Where(em => em.MissionId == missionId)
            .Select(em => em.EquipmentId)
            .ToListAsync();


        if (mission == null)
        {
            return false; // Mission not found
        }

        if (!mission.AssignedEquipments.Contains(equipmentId))
        {
            return false; // Equipment not assigned to this mission
        }

        // Remove the equipment from the AssignedEquipments list
        mission.AssignedEquipments.Remove(equipmentId);

        // Optional: If you have a table for EquipmentMission, remove the entry as well
        var equipmentMission = await _context.EquipmentMissions
            .FirstOrDefaultAsync(em => em.MissionId == missionId && em.EquipmentId == equipmentId);

        if (equipmentMission != null)
        {
            _context.EquipmentMissions.Remove(equipmentMission);
        }

        _context.Missions.Update(mission); // Update the mission

        // Save changes to the database
        return await _context.SaveChangesAsync() > 0;
    }


    // Get missions for a specific admin by AdminId, including related accounts/equipment
    public async Task<List<MissionDTO>> GetMissionsByAdminIdAsync(int adminId)
    {
        return await _context.Missions
            .Where(m => m.AdminId == adminId)
            .Select(m => new MissionDTO
            {
                Id = m.Id,
                Description = m.Description,
                Type = m.Type,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Location = m.Location,
                Status = m.Status,
                AdminId = m.AdminId
            })
            .ToListAsync();
    }

    public Task<List<Equipment>> GetAllEquipmentAsync()
    {
        throw new NotImplementedException();
    }
}
