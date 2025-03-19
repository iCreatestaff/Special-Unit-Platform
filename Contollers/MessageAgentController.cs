using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sp_backend_March4.Interfaces;
using sp_backend_March4.DTO;

namespace sp_backend_March4.Contollers
{
    [Route("api/messages")]
    [ApiController]
    public class MessageAgentController : ControllerBase
    {
        private readonly IMessageAgentService _messageService;

        public MessageAgentController(IMessageAgentService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var message = await _messageService.SendMessageAsync(request.SenderId, request.ReceiverId, request.Content);
            return Ok(message);
        }

        [HttpGet("agent/{agentId}")]
        public async Task<IActionResult> GetMessagesForAgent(int agentId)
        {
            var messages = await _messageService.GetMessagesForAgentAsync(agentId);
            return Ok(messages);
        }

        [HttpGet("{messageId}")]
        public async Task<IActionResult> GetMessageById(int messageId)
        {
            var message = await _messageService.GetMessageByIdAsync(messageId);
            if (message == null)
                return NotFound();

            return Ok(message);
        }

        [HttpPut("read/{messageId}")]
        public async Task<IActionResult> MarkMessageAsRead(int messageId)
        {
            await _messageService.MarkMessageAsReadAsync(messageId);
            return NoContent();
        }

        [HttpPut("{userId}/mark-read")]
        public async Task<IActionResult> MarkAllMessagesAsRead(int userId)
        {
            await _messageService.MarkAllMessagesAsReadAsync(userId);
            return NoContent(); // 204 No Content (successful update, no response body)
        }


    }

}