using System;

namespace sp_backend.DTO
{
    public class NonAvailabilityDTO
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }
    }
}
