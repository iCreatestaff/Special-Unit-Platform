using sp_backend.DTO;
using sp_backend.Models;  // Assuming Equipment is in the Models namespace
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApi.Models;

namespace sp_backend.Interfaces
{
    public interface IMissionService
    {
        Task<bool> CreateMissionAsync(MissionDTO missionDTO);
        Task<MissionDTO?> GetMissionByIdAsync(int id);
        Task<List<MissionDTO>> GetAllMissionsAsync();
        Task<bool> UpdateMissionAsync(int id, MissionDTO missionDTO);
        Task<bool> DeleteMissionAsync(int id);

        // New method to get all equipment
        Task<List<Equipment>> GetAllEquipmentAsync();  // Ensure Equipment is the correct type

    }
}
