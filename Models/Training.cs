using System;
using System.Collections.Generic;
using sp_backend_March4.Models;

namespace sp_backend.Models
{
    public class Training
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";

        public List<AccountTraining> AccountTrainings { get; set; } = new();
    }

}
