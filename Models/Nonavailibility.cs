using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sp_backend.Models
{
    public class Nonavailability
    {
        public int Id { get; set; }
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }

}