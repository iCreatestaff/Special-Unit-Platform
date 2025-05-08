using Microsoft.AspNetCore.Mvc;
using sp_backend.DTO;
using sp_backend.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sp_backend.Controllers
{
    [ApiController]
    [Route("api/nonavailability")]
    public class NonAvailabilityController : ControllerBase
    {
        private readonly INonavailabilityService _nonavailabilityService;

        public NonAvailabilityController(INonavailabilityService nonavailabilityService)
        {
            _nonavailabilityService = nonavailabilityService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateNonAvailability([FromBody] NonAvailabilityDTO nonAvailabilityDTO)
        {
            if (nonAvailabilityDTO.Date1 >= nonAvailabilityDTO.Date2)
            {
                return BadRequest("The start date (Date1) must be before the end date (Date2).");
            }

            // Ensure only one type of non-availability is provided
            if ((nonAvailabilityDTO.AccountId == null && nonAvailabilityDTO.EquipmentId == null) ||
                (nonAvailabilityDTO.AccountId != null && nonAvailabilityDTO.EquipmentId != null))
            {
                return BadRequest("NonAvailability must be associated with either an Account or a Equipment, but not both.");
            }

            var result = await _nonavailabilityService.CreateNonAvailabilityAsync(nonAvailabilityDTO);
            return result ? Ok("NonAvailability created successfully") : BadRequest("Failed to create NonAvailability.");
        }

        [HttpGet("account/{accountId}")]
        public async Task<ActionResult<List<NonAvailabilityDTO>>> GetNonAvailabilityByAccountId(int accountId)
        {
            var nonAvailabilityList = await _nonavailabilityService.GetNonAvailabilityByAccountIdAsync(accountId);
            return Ok(nonAvailabilityList);
        }

        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByType(string type)
        {
            var notifications = await _notificationService.GetByTypeAsync(type);
            return Ok(notifications);
        }

        [HttpGet("Equipment/{EquipmentId}")]
        public async Task<ActionResult<List<NonAvailabilityDTO>>> GetNonAvailabilityBySubEquipmentId(int EquipmentId)
        {
            var nonAvailabilityList = await _nonavailabilityService.GetNonAvailabilityByEquipmentIdAsync(EquipmentId);
            return Ok(nonAvailabilityList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNonAvailability(int id, [FromBody] NonAvailabilityDTO nonAvailabilityDTO)
        {
            var result = await _nonavailabilityService.UpdateNonAvailabilityAsync(id, nonAvailabilityDTO);
            return result ? Ok("NonAvailability updated successfully") : NotFound("NonAvailability not found or invalid update.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNonAvailability(int id)
        {
            var result = await _nonavailabilityService.DeleteNonAvailabilityAsync(id);
            return result ? Ok("NonAvailability deleted successfully") : NotFound("NonAvailability not found.");
        }
    }
}
