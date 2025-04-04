using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using sp_backend_March4.Interfaces;
using sp_backend_March4.Models;

namespace sp_backend_March4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestMaintenanceController : ControllerBase
    {
        private readonly IRequestMaintenanceService _service;
        private readonly ILogger<RequestMaintenanceController> _logger;

        public RequestMaintenanceController(IRequestMaintenanceService service, ILogger<RequestMaintenanceController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequestMaintenance([FromBody] RequestMaintenance request)
        {
            if (request == null)
            {
                return BadRequest("Request body cannot be null.");
            }

            try
            {
                var createdRequest = await _service.CreateRequestMaintenanceAsync(request);
                return CreatedAtAction(nameof(GetRequestMaintenanceById), new { id = createdRequest.Id }, createdRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RequestMaintenance.");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequestMaintenanceById(int id)
        {
            try
            {
                var request = await _service.GetRequestMaintenanceByIdAsync(id);
                if (request == null) return NotFound($"RequestMaintenance with ID {id} not found.");
                return Ok(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving RequestMaintenance with ID {id}.");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRequestMaintenances()
        {
            try
            {
                var requests = await _service.GetAllRequestMaintenancesAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all RequestMaintenances.");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateRequestMaintenanceStatus(int id, [FromBody] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("Status cannot be empty.");
            }

            try
            {
                var updatedRequest = await _service.UpdateRequestMaintenanceStatusAsync(id, status);
                if (updatedRequest == null) return NotFound($"RequestMaintenance with ID {id} not found.");
                return Ok(updatedRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating status of RequestMaintenance with ID {id}.");
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }

    }
}
