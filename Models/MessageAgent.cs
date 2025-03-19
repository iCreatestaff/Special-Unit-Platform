using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace sp_backend_March4.Models
{
    public class MessageAgent
    {
        public int Id { get; set; }

        [ForeignKey(nameof(Sender))]
        public int SenderId { get; set; }
        public Account Sender { get; set; }

        [ForeignKey(nameof(Receiver))]
        public int ReceiverId { get; set; }
        public Account Receiver { get; set; }

        public string Content { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        // Navigation properties (optional, if using Entity Framework)


    }

}