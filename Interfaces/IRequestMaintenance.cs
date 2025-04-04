using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sp_backend_March4.Models;

namespace sp_backend_March4.Interfaces
{
    public interface IRequestMaintenanceService
    {
        Task<RequestMaintenance> CreateRequestMaintenanceAsync(RequestMaintenance request);
        Task<RequestMaintenance?> GetRequestMaintenanceByIdAsync(int id);
        Task<IEnumerable<RequestMaintenance>> GetAllRequestMaintenancesAsync();
        Task<RequestMaintenance> UpdateRequestMaintenanceStatusAsync(int id, string status);
    }

}