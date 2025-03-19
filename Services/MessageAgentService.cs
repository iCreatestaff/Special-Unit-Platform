using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sp_backend_March4.DTO;
using sp_backend_March4.Interfaces;
using sp_backend_March4.Models;
using WeatherApi;

namespace sp_backend_March4.Services
{
    public class MessageAgentService : IMessageAgentService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MessageAgentService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MessageAgent> SendMessageAsync(int senderId, int receiverId, string content)
        {
            var message = new MessageAgent
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            _context.MessageAgents.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<MessageAgent>> GetMessagesForAgentAsync(int agentId)
        {
            return await _context.MessageAgents
                .Where(m => m.ReceiverId == agentId)
                .OrderByDescending(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task MarkAllMessagesAsReadAsync(int agentId)
        {
            var unreadMessages = await _context.MessageAgents
                .Where(m => m.ReceiverId == agentId && !m.IsRead)
                .ToListAsync();

            if (!unreadMessages.Any())
                return; // No unread messages, return early.

            // Update all unread messages
            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<MessageAgent> GetMessageByIdAsync(int messageId)
        {
            return await _context.MessageAgents.FindAsync(messageId);
        }

        public async Task MarkMessageAsReadAsync(int messageId)
        {
            var message = await _context.MessageAgents.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }

}