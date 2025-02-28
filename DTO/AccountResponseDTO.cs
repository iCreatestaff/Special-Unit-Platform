using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sp_backend.Models;

namespace sp_backend.DTO
{
    public class AccountResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Type { get; set; }
        public string Badge { get; set; }
        public string? Password { get; set; } // Used only for creation
        public string Role { get; set; }
        public string? SocialFile { get; set; }
        public string? MedicalFile { get; set; }
        public string? CareerFile { get; set; }

        public Account ToEntity(string passwordHash)
        {
            return new Account
            {
                Id = Id,
                Username = Username,
                Name = Name,
                Type = Type,
                Badge = Badge,
                PasswordHash = passwordHash, // Corrected this
                Role = Role,
                SocialFile = SocialFile,
                MedicalFile = MedicalFile,
                CareerFile = CareerFile
            };
        }
    }


}