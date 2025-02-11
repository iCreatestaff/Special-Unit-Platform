using sp_backend.DTO;
using sp_backend.Models;

namespace sp_backend.Mappers
{
    public static class AccountMapper
    {
        // Mapping Account entity to AccountResponseDTO
        public static AccountResponseDTO ToDto(this Account account)
        {
            return new AccountResponseDTO
            {
                Id = account.Id,
                Name = account.Name,
                Username = account.Username,
                Role = account.Role,
                Photo = account.Photo,

            };
        }

        // Mapping AccountDTO to Account entity
        public static Account ToEntity(this AccountDTO dto, string passwordHash)
        {
            return new Account
            {
                Id = dto.Id,
                Name = dto.Name,
                Username = dto.Username,
                PasswordHash = passwordHash,
                Role = dto.Role,
                Photo = dto.Photo,
                SocialFile = dto.SocialFile,
                MedicalFile = dto.MedicalFile,
                CareerFile = dto.CareerFile
            };
        }
    }
}
