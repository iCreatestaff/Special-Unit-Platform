using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sp_backend.DTO;

namespace sp_backend.Interfaces
{
    public interface INonavailabilityService
    {
        Task<List<NonAvailabilityDTO>> GetNonAvailabilityByAccountIdAsync(int accountId);
        Task<bool> CreateNonAvailabilityAsync(NonAvailabilityDTO nonAvailabilityDTO);
        Task<bool> UpdateNonAvailabilityAsync(int id, NonAvailabilityDTO nonAvailabilityDTO);
        Task<bool> DeleteNonAvailabilityAsync(int id);
        Task<List<NonAvailabilityDTO>> GetNonAvailabilityBySubEquipmentIdAsync(int subEquipmentId);
    }
}
