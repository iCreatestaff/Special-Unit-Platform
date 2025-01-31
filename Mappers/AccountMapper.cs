using sp_backend.DTO;
using sp_backend.Models;

namespace sp_backend.Mappers
{
    public static class AccountMapper
    {
        // Mapping Account entity to AccountResponseDTO, now includes AssignedMissions
        public static AccountResponseDTO ToDto(this Account account)
        {
            return new AccountResponseDTO
            {
                Id = account.Id,
                Name = account.Name,
                Username = account.Username,
                Role = account.Role,
                AssignedMissions = account.Missions?.Select(m => m.Id).ToList() // Using null-conditional operator in case Missions is null
            };
        }

        // Mapping AccountCreateDTO to Account entity
        public static Account ToEntity(this AccountCreateDTO dto, string passwordHash)
        {
            return new Account
            {
                Name = dto.Name,
                Username = dto.Username,
                PasswordHash = passwordHash,
                Role = dto.Role
            };
        }
    }
}
