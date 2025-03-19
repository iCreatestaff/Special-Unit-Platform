using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sp_backend_March4.DTO
{
    public class SendMessageRequest
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; }
    }

}