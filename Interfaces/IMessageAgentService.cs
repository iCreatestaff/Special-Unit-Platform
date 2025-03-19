using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sp_backend_March4.DTO;
using sp_backend_March4.Models;

namespace sp_backend_March4.Interfaces
{
    public interface IMessageAgentService
    {
        Task<MessageAgent> SendMessageAsync(int senderId, int receiverId, string content);
        Task<IEnumerable<MessageAgent>> GetMessagesForAgentAsync(int agentId);
        Task MarkAllMessagesAsReadAsync(int agentId);
        Task<MessageAgent> GetMessageByIdAsync(int messageId);
        Task MarkMessageAsReadAsync(int messageId);
    }

}