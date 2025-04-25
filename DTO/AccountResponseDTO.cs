using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sp_backend.Models;
using sp_backend_March4.DTO;
using sp_backend_March4.Models;

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
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<AccountTraining> AccountTrainings { get; set; } = new();
        public List<MessageDto> SentMessages { get; set; } = new();
        public List<MessageDto> ReceivedMessages { get; set; } = new();

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
                Latitude = Latitude,
                Longitude = Longitude,
                MedicalFile = MedicalFile,
                CareerFile = CareerFile,
                AccountTrainings = AccountTrainings,
                ReceivedMessages = (ICollection<MessageAgent>)ReceivedMessages,
                SentMessages = (ICollection<MessageAgent>)SentMessages
            };
        }
    }


}