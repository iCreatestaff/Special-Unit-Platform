using Microsoft.AspNetCore.Mvc;
using WeatherApi.Interfaces;
using WeatherApi.Models;
using WeatherApi.DTOs;
using AutoMapper;
using sp_backend.DTO;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;
        private readonly IMapper _mapper;

        public EquipmentController(IEquipmentService equipmentService, IMapper mapper)
        {
            _equipmentService = equipmentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipmentDto>>> GetEquipments()
        {
            var equipments = await _equipmentService.GetAllEquipmentsAsync();
            var equipmentDtos = _mapper.Map<IEnumerable<EquipmentDto>>(equipments);
            return Ok(equipmentDtos);
        }
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableEquipment()
        {
            var availableEquipment = await _equipmentService.GetAvailableEquipmentAsync();

            if (availableEquipment == null || availableEquipment.Count == 0)
            {
                return NotFound(new { Message = "No available equipment found." });
            }

            return Ok(availableEquipment);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentDto>> GetEquipment(int id)
        {
            var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
            if (equipment == null)
            {
                return NotFound();
            }
            var equipmentDto = _mapper.Map<EquipmentDto>(equipment);
            return Ok(equipmentDto);
        }

        [HttpPost]
        public async Task<ActionResult<EquipmentDto>> CreateEquipment([FromBody] EquipmentDto equipmentDto)
        {
            try
            {
                var equipment = _mapper.Map<Equipment>(equipmentDto);
                var createdEquipment = await _equipmentService.CreateEquipmentAsync(equipment);
                var createdEquipmentDto = _mapper.Map<EquipmentDto>(createdEquipment);

                return CreatedAtAction(nameof(GetEquipment), new { id = createdEquipmentDto.Id }, createdEquipmentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("create-with-quantity")]
        public async Task<ActionResult<List<EquipmentDto>>> CreateEquipmentWithQuantity(
    [FromBody] EquipmentWithQuantityDto request)
        {
            if (request.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }
            try
            {
                var equipment = _mapper.Map<Equipment>(request.Equipment);

                var createdEquipments = await _equipmentService.CreateEquipmentWithQuantityAsync(equipment, request.Quantity);

                var createdEquipmentsDto = _mapper.Map<List<EquipmentDto>>(createdEquipments);

                return Ok(createdEquipmentsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }



        [HttpPut("{id}")]
        public async Task<ActionResult<EquipmentDto>> UpdateEquipment(int id, [FromBody] EquipmentDto equipmentDto)
        {
            var equipment = _mapper.Map<Equipment>(equipmentDto);
            var updatedEquipment = await _equipmentService.UpdateEquipmentAsync(id, equipment);
            if (updatedEquipment == null)
            {
                return NotFound();
            }
            var updatedEquipmentDto = _mapper.Map<EquipmentDto>(updatedEquipment);
            return Ok(updatedEquipmentDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            var result = await _equipmentService.DeleteEquipmentAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
