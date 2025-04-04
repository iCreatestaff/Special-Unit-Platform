using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using sp_backend.Models;
using WeatherApi.Models;

namespace sp_backend_March4.Models
{
    public class RequestMaintenance
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MaintenanceId { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Possible values: "Pending", "Accepted", "Rejected"

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;
        public int? EquipmentId { get; internal set; }

        [ForeignKey("MaintenanceId")]
        public Maintenance Maintenance { get; set; }
    }

}