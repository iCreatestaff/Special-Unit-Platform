using Microsoft.EntityFrameworkCore;
using sp_backend.DTO;
using sp_backend.Interfaces;
using WeatherApi;

namespace sp_backend.Services
{
    public class NonavailabilityService : INonavailabilityService
    {
        private readonly AppDbContext _context;

        public NonavailabilityService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NonAvailabilityDTO>> GetNonAvailabilityByAccountIdAsync(int accountId)
        {
            var nonAvailabilityList = await _context.Nonavailabilities
                .Where(na => na.AccountId == accountId)
                .ToListAsync();

            return nonAvailabilityList.Select(na => new NonAvailabilityDTO
            {
                Id = na.Id,
                AccountId = na.AccountId,
                Date1 = na.Date1,
                Date2 = na.Date2
            }).ToList();
        }

        public async Task<bool> CreateNonAvailabilityAsync(NonAvailabilityDTO nonAvailabilityDTO)
        {
            var nonAvailability = new Nonavailability
            {
                AccountId = nonAvailabilityDTO.AccountId,
                Date1 = nonAvailabilityDTO.Date1,
                Date2 = nonAvailabilityDTO.Date2
            };

            await _context.Nonavailabilities.AddAsync(nonAvailability);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateNonAvailabilityAsync(int id, NonAvailabilityDTO nonAvailabilityDTO)
        {
            var nonAvailability = await _context.Nonavailabilities.FindAsync(id);
            if (nonAvailability == null)
            {
                return false; // Not found
            }

            nonAvailability.Date1 = nonAvailabilityDTO.Date1;
            nonAvailability.Date2 = nonAvailabilityDTO.Date2;

            _context.Nonavailabilities.Update(nonAvailability);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNonAvailabilityAsync(int id)
        {
            var nonAvailability = await _context.Nonavailabilities.FindAsync(id);
            if (nonAvailability == null)
            {
                return false; // Not found
            }

            _context.Nonavailabilities.Remove(nonAvailability);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
