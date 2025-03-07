using System;
using System.Collections.Generic;

namespace sp_backend.Models
{
    public class Training
    {
        public int Id { get; set; }  // Unique identifier
        public string Title { get; set; } = string.Empty;  // Training name
        public string? Description { get; set; }  // Training details (optional)
        public DateTime StartTime { get; set; }  // Start date and time
        public DateTime EndTime { get; set; }  // End date and time
        public string Location { get; set; } = string.Empty;  // Training location
        public List<int> Participants { get; set; } = new();  // List of agent/admin IDs
        public TrainingStatus Status { get; set; } = TrainingStatus.Scheduled;  // Training status
    }

    public enum TrainingStatus
    {
        Scheduled,
        Ongoing,
        Completed,
        Canceled
    }
}
