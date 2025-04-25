using System.Collections.Generic;
using sp_backend.Models;
using sp_backend_March4.Models;

namespace sp_backend.DTO
{
    public class AccountDTO
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
        public string? Photo { get; set; }
        public List<AccountTraining>? AccountTrainings { get; set; } = new();
        public ICollection<MessageAgent>? SentMessages { get; set; }
        public ICollection<MessageAgent>? ReceivedMessages { get; set; }

        public Account ToEntity(string passwordHash)
        {
            return new Account
            {
                Id = Id,
                Username = Username,
                Name = Name,
                Type = Type,
                Badge = Badge,
                PasswordHash = passwordHash,
                Role = Role,
                SocialFile = SocialFile,
                MedicalFile = MedicalFile,
                CareerFile = CareerFile,
                Latitude = Latitude,
                Longitude = Longitude,
                Photo = Photo,
                AccountTrainings = AccountTrainings,
                ReceivedMessages = ReceivedMessages,
                SentMessages = SentMessages
            };
        }
    }
}
