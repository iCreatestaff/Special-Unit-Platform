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
            StartTime = missionDTO.StartTime,
            EndTime = missionDTO.EndTime,
            Location = missionDTO.Location,
            Status = missionDTO.Status,
            AdminId = missionDTO.AdminId
        };

        _context.Missions.Add(mission);
        await _context.SaveChangesAsync();  // Save to generate MissionId

        // Create many-to-many relationships
        var accountMissions = missionDTO.AssignedAccountIds
            .Select(accountId => new AccountMission { AccountId = accountId, MissionId = mission.Id })
            .ToList();

        _context.AccountMissions.AddRange(accountMissions);
        return await _context.SaveChangesAsync() > 0;
    }


    // Get mission by ID, including related accounts and equipment
    public async Task<MissionDTO?> GetMissionByIdAsync(int id)
    {
        var mission = await _context.Missions
            .Include(m => m.AssignedAccounts)   // Eagerly load related accounts
            .Include(m => m.AssignedEquipment)  // Eagerly load related equipment
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mission == null) return null;

        return new MissionDTO
        {
            Id = mission.Id,
            Description = mission.Description,
            StartTime = mission.StartTime,
            EndTime = mission.EndTime,
            Location = mission.Location,
            Status = mission.Status,
            AdminId = mission.AdminId,
            AssignedAccountIds = mission.AssignedAccounts.Select(a => a.Id).ToList(),
            AssignedEquipmentIds = mission.AssignedEquipment.Select(e => e.Id).ToList()
        };
    }

    // Get all missions, including related accounts and equipment
    public async Task<List<MissionDTO>> GetAllMissionsAsync()
    {
        return await _context.Missions
            .Include(m => m.AssignedAccounts)   // Eagerly load related accounts
            .Include(m => m.AssignedEquipment)  // Eagerly load related equipment
            .Select(m => new MissionDTO
            {
                Id = m.Id,
                Description = m.Description,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Location = m.Location,
                Status = m.Status,
                AdminId = m.AdminId,
                AssignedAccountIds = m.AssignedAccounts.Select(a => a.Id).ToList(),
                AssignedEquipmentIds = m.AssignedEquipment.Select(e => e.Id).ToList()
            })
            .ToListAsync();
    }

    // Update an existing mission and associate it with updated accounts/equipment
    public async Task<bool> UpdateMissionAsync(int id, MissionDTO missionDTO)
    {
        var mission = await _context.Missions.FindAsync(id);
        if (mission == null) return false;

        mission.Description = missionDTO.Description;
        mission.StartTime = missionDTO.StartTime;
        mission.EndTime = missionDTO.EndTime;
        mission.Location = missionDTO.Location;
        mission.Status = missionDTO.Status;
        mission.AdminId = missionDTO.AdminId;
        mission.AssignedAccounts = await _context.Accounts
            .Where(a => missionDTO.AssignedAccountIds.Contains(a.Id))
            .ToListAsync();
        mission.AssignedEquipment = await _context.Equipments
            .Where(e => missionDTO.AssignedEquipmentIds.Contains(e.Id))
            .ToListAsync();

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
            .Include(m => m.AssignedEquipment) // Eagerly load related equipment
            .Select(m => new MissionDTO
            {
                Id = m.Id,
                Description = m.Description,
                StartTime = m.StartTime,
                EndTime = m.EndTime,
                Location = m.Location,
                Status = m.Status,
                AdminId = m.AdminId,
                AssignedAccountIds = m.AssignedAccounts.Select(a => a.Id).ToList(),
                AssignedEquipmentIds = m.AssignedEquipment.Select(e => e.Id).ToList()
            })
            .ToListAsync();
    }

    public Task<List<Equipment>> GetAllEquipmentAsync()
    {
        throw new NotImplementedException();
    }
}
