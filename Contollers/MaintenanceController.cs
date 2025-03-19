using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sp_backend.DTO;
using sp_backend.Models;
using WeatherApi.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService;
        private readonly IMapper _mapper;

        public MaintenanceController(IMaintenanceService maintenanceService, IMapper mapper)
        {
            _maintenanceService = maintenanceService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMaintenances()
        {
            var maintenances = await _maintenanceService.GetAllMaintenancesAsync();
            return Ok(_mapper.Map<List<MaintenanceDTO>>(maintenances));
        }

        [HttpGet("grouped")]
        public async Task<IActionResult> GetGroupedMaintenances()
        {
            var result = await _maintenanceService.GetGroupedMaintenancesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaintenanceById(int id)
        {
            var maintenance = await _maintenanceService.GetMaintenanceByIdAsync(id);
            if (maintenance == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<MaintenanceDTO>(maintenance));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaintenance([FromBody] MaintenanceDTO maintenanceDTO)
        {
            if (maintenanceDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            try
            {
                var maintenance = _mapper.Map<Maintenance>(maintenanceDTO);
                var createdMaintenance = await _maintenanceService.CreateMaintenanceAsync(maintenance);

                return CreatedAtAction(
                    nameof(GetMaintenanceById),
                    new { id = createdMaintenance.Id },
                    _mapper.Map<MaintenanceDTO>(createdMaintenance)
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred while creating the maintenance record.");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaintenance(int id, [FromBody] MaintenanceDTO maintenanceDTO)
        {
            var existingMaintenance = await _maintenanceService.GetMaintenanceByIdAsync(id);
            if (existingMaintenance == null)
            {
                return NotFound();
            }

            var maintenanceToUpdate = _mapper.Map<Maintenance>(maintenanceDTO);
            maintenanceToUpdate.Id = id; // Ensure ID remains the same

            var updatedMaintenance = await _maintenanceService.UpdateMaintenanceAsync(id, maintenanceToUpdate);
            return Ok(_mapper.Map<MaintenanceDTO>(updatedMaintenance));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaintenance(int id)
        {
            var deleted = await _maintenanceService.DeleteMaintenanceAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
