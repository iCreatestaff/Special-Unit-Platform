using Microsoft.AspNetCore.Mvc;
using WeatherApi.Interfaces;
using WeatherApi.Models;
using WeatherApi.DTOs;
using AutoMapper;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubEquipmentController : ControllerBase
    {
        private readonly ISubEquipmentService _subEquipmentService;
        private readonly IMapper _mapper;

        public SubEquipmentController(ISubEquipmentService subEquipmentService, IMapper mapper)
        {
            _subEquipmentService = subEquipmentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubEquipmentDto>>> GetSubEquipments()
        {
            var subEquipments = await _subEquipmentService.GetAllSubEquipmentsAsync();
            var subEquipmentDtos = _mapper.Map<IEnumerable<SubEquipmentDto>>(subEquipments);
            return Ok(subEquipmentDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SubEquipmentDto>> GetSubEquipment(int id)
        {
            var subEquipment = await _subEquipmentService.GetSubEquipmentByIdAsync(id);
            if (subEquipment == null)
            {
                return NotFound();
            }
            var subEquipmentDto = _mapper.Map<SubEquipmentDto>(subEquipment);
            return Ok(subEquipmentDto);
        }

        [HttpPost]
        public async Task<ActionResult<SubEquipmentDto>> CreateSubEquipment([FromBody] SubEquipmentDto subEquipmentDto)
        {
            var subEquipment = _mapper.Map<SubEquipment>(subEquipmentDto);
            subEquipment.CreationDate = DateTime.UtcNow;
            var createdSubEquipment = await _subEquipmentService.CreateSubEquipmentAsync(subEquipment);
            var createdSubEquipmentDto = _mapper.Map<SubEquipmentDto>(createdSubEquipment);
            return CreatedAtAction(nameof(GetSubEquipment), new { id = createdSubEquipmentDto.Id }, createdSubEquipmentDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SubEquipmentDto>> UpdateSubEquipment(int id, [FromBody] SubEquipmentDto subEquipmentDto)
        {
            var subEquipment = _mapper.Map<SubEquipment>(subEquipmentDto);
            // Ensure the CreationDate is not modified during the update
            var existingSubEquipment = await _subEquipmentService.GetSubEquipmentByIdAsync(id);
            if (existingSubEquipment == null)
            {
                return NotFound();
            }

            // Keep the existing CreationDate unchanged
            subEquipment.CreationDate = existingSubEquipment.CreationDate;
            var updatedSubEquipment = await _subEquipmentService.UpdateSubEquipmentAsync(id, subEquipment);
            if (updatedSubEquipment == null)
            {
                return NotFound();
            }
            var updatedSubEquipmentDto = _mapper.Map<SubEquipmentDto>(updatedSubEquipment);
            return Ok(updatedSubEquipmentDto);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubEquipment(int id)
        {
            var result = await _subEquipmentService.DeleteSubEquipmentAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
