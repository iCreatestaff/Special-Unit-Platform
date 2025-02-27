using WeatherApi.DTOs;
using sp_backend.Models;

namespace WeatherApi.Interfaces
{
    public interface IMaintenanceService
    {
        Task<IEnumerable<Maintenance>> GetAllMaintenancesAsync();
        Task<Maintenance?> GetMaintenanceByIdAsync(int id);
        Task<Maintenance> CreateMaintenanceAsync(Maintenance maintenance);
        Task<Maintenance?> UpdateMaintenanceAsync(int id, Maintenance maintenance);
        Task<bool> DeleteMaintenanceAsync(int id);
    }
}
