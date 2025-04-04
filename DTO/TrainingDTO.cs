using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sp_backend_March4.Models;

namespace sp_backend_March4.DTO
{
    public class TrainingDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Example: Scheduled, Completed, Cancelled
        public List<int> AssignedAccounts { get; set; } = new();
        public List<AccountTraining> AccountTrainings { get; set; } = new();
    }


}