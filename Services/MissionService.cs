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
        var mission = new Mission
        {
            Description = missionDTO.Description,
            Type = missionDTO.Type,
            StartTime = missionDTO.StartTime,
            EndTime = missionDTO.EndTime,
            Location = missionDTO.Location,
            Status = missionDTO.Status,
            AdminId = missionDTO.AdminId
        };

        _context.Missions.Add(mission);

        // Save mission first to get its ID
        await _context.SaveChangesAsync();

        // Create AccountMission entries
        if (missionDTO.AssignedAccounts != null)
        {
            foreach (var accountId in missionDTO.AssignedAccounts)
            {
                var accountMission = new AccountMission
                {
                    MissionId = mission.Id,
                    AccountId = accountId
                };
                _context.AccountMissions.Add(accountMission);
            }
        }

        // Create EquipmentMission entries
        if (missionDTO.AssignedEquipments != null)
        {
            foreach (var equipmentId in missionDTO.AssignedEquipments)
            {
                var equipmentMission = new EquipmentMission
                {
                    MissionId = mission.Id,
                    EquipmentId = equipmentId
                };
                _context.EquipmentMissions.Add(equipmentMission);
            }
        }

        // Save changes for AccountMission and EquipmentMission
        return await _context.SaveChangesAsync() > 0;
    }





    // Get mission by ID, including related accounts and equipment
    public async Task<MissionDTO?> GetMissionByIdAsync(int id)
    {
        var mission = await _context.Missions
            .Include(m => m.AccountMissions) // Load AccountMission relations
            .Include(m => m.EquipmentMissions) // Load EquipmentMission relations
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


    // Get all missions, including related accounts and equipment
    public async Task<List<MissionDTO>> GetAllMissionsAsync()
    {
        return await _context.Missions
            .Include(m => m.AccountMissions) // Load AccountMission relations
            .Include(m => m.EquipmentMissions) // Load EquipmentMission relations
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


    // Update an existing mission and associate it with updated accounts/equipment
    public async Task<bool> UpdateMissionAsync(int id, MissionDTO missionDTO)
    {
        var mission = await _context.Missions
            .Include(m => m.AccountMissions) // Load current AccountMissions
            .Include(m => m.EquipmentMissions) // Load current EquipmentMissions
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

        // Add new assigned accounts
        foreach (var accountId in newAccountIds.Except(existingAccountIds))
        {
            mission.AccountMissions.Add(new AccountMission { MissionId = id, AccountId = accountId });
        }

        // Handle assigned equipment
        var existingEquipmentIds = mission.EquipmentMissions.Select(em => em.EquipmentId).ToList();
        var newEquipmentIds = missionDTO.AssignedEquipments ?? new List<int>();

        // Remove equipment that is no longer assigned
        mission.EquipmentMissions.RemoveAll(em => !newEquipmentIds.Contains(em.EquipmentId));

        // Add new assigned equipment
        foreach (var equipmentId in newEquipmentIds.Except(existingEquipmentIds))
        {
            mission.EquipmentMissions.Add(new EquipmentMission { MissionId = id, EquipmentId = equipmentId });
        }

        return await _context.SaveChangesAsync() > 0;
    }


    // Delete a mission by ID
    public async Task<bool> DeleteMissionAsync(int id)
    {
        var mission = await _context.Missions.FindAsync(id);
        if (mission == null) return false;

        _context.Missions.Remove(mission);
        return await _context.SaveChangesAsync() > 0;
    }

    // Get missions for a specific admin by AdminId, including related accounts/equipment
    public async Task<List<MissionDTO>> GetMissionsByAdminIdAsync(int adminId)
    {
        return await _context.Missions
            .Where(m => m.AdminId == adminId)  // Filter by AdminId
            .Include(m => m.AssignedAccounts)  // Eagerly load related accounts
            .Include(m => m.AssignedEquipments) // Eagerly load related equipment
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
            })
            .ToListAsync();
    }

    public Task<List<Equipment>> GetAllEquipmentAsync()
    {
        throw new NotImplementedException();
    }
}
