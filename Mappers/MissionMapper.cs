using sp_backend.DTO;
using sp_backend.Models;
using System.Linq;
using System.Collections.Generic;  // Ensure this is included
using WeatherApi.Models;

public static class MissionMapper
{
    // Mapping Mission entity to MissionDTO
    public static MissionDTO ToDto(this Mission mission)
    {
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

    // Mapping MissionCreateDTO to Mission entity
    public static Mission ToEntity(this MissionCreateDTO dto, List<Account> accounts, List<Equipment> equipment)
    {
        var mission = new Mission
        {
            Description = dto.Description,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Location = dto.Location,
            Status = dto.Status,
            AdminId = dto.AdminId,
            // Initialize lists for AssignedAccounts and AssignedEquipment
            AssignedAccounts = new List<Account>(),
            AssignedEquipment = new List<Equipment>()
        };

        // Assigning accounts that are part of the mission (based on IDs from dto)
        mission.AssignedAccounts = accounts.Where(a => dto.AssignedAccountIds.Contains(a.Id)).ToList();

        // Assigning equipment that is part of the mission (based on IDs from dto)
        mission.AssignedEquipment = equipment.Where(e => dto.AssignedEquipmentIds.Contains(e.Id)).ToList();

        return mission;
    }
}
