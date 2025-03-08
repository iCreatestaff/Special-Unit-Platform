using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using sp_backend.Models;

namespace sp_backend_March4.Models
{
    public class AccountTraining
    {
        public int AccountId { get; set; }
        [JsonIgnore]
        public Account? Account { get; set; } = null!;
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public int TrainingId { get; set; }
        [JsonIgnore]
        public Training? Training { get; set; } = null!;
    }
}