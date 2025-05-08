using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sp_backend_March4.DTO;
using sp_backend_March4.Interfaces;

namespace sp_backend_March4.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAllNotifications()
        {
            var notifications = await _notificationService.GetAllNotificationsAsync();
            return Ok(notifications);
        }
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByType(string type)
        {
            var notifications = await _notificationService.GetByTypeAsync(type);
            return Ok(notifications);
        }

        [HttpPost]
        public async Task<ActionResult<NotificationDto>> CreateNotification(CreateNotificationDto dto)
        {
            var result = await _notificationService.CreateNotificationAsync(dto);
            return Ok(result);
        }

        [HttpGet("recipient/{recipientId}")]
        public async Task<ActionResult<List<NotificationDto>>> GetByRecipient(int recipientId)
        {
            var result = await _notificationService.GetNotificationsByRecipientIdAsync(recipientId);
            return Ok(result);
        }

        [HttpPut("read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var result = await _notificationService.MarkAsReadAsync(notificationId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> Delete(int notificationId)
        {
            var result = await _notificationService.DeleteNotificationAsync(notificationId);
            if (!result) return NotFound();
            return NoContent();
        }
    }

}