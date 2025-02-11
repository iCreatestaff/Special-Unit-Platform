using sp_back.Migrations;
using sp_backend.DTO;
using sp_backend.Models;

namespace sp_backend.Mappers
{
    public static class AccountMapper
    {
        // Mapping Account entity to AccountResponseDTO
        public static AccountDTO ToDto(this Account account)
        {
            return new AccountDTO
            {
                Id = account.Id,
                Name = account.Name,
                Username = account.Username,
                Type = account.Type,
                Password = account.PasswordHash,
                Role = account.Role,
                Photo = account.Photo,
                SocialFile = account.SocialFile,
                MedicalFile = account.MedicalFile,
                CareerFile = account.CareerFile

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
                Type = dto.Type,
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
