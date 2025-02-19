using System;

namespace sp_backend.DTO
{
    public class NonAvailabilityDTO
    {
        public int Id { get; set; }

        // Type field to indicate whether it's for an Account or SubEquipment
        public string? Type { get; set; } // Values: "Account" or "SubEquipment"

        // Foreign key references (only one should be set)
        public int? AccountId { get; set; }
        public int? EquipmentId { get; set; }

        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }
    }
}
