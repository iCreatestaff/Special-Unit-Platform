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
        public string Role { get; set; }
        public string Type { get; set; }
        public string? Photo { get; set; }
        public List<AccountMission> AccountMissions { get; set; } = new();

    }


}