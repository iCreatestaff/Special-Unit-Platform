using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sp_backend.DTO;
using sp_backend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeatherApi.DTOs;
using WeatherApi.Models;

namespace sp_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentStockController : ControllerBase
    {
        private readonly IEquipmentStockService _equipmentStockService;
        private readonly IMapper _mapper;

        public EquipmentStockController(IEquipmentStockService equipmentStockService, IMapper mapper)
        {
            _equipmentStockService = equipmentStockService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipmentStockDTO>>> GetAll()
        {
            var result = await _equipmentStockService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipmentStockDTO>> GetById(int id)
        {
            var equipmentStock = await _equipmentStockService.GetByIdAsync(id);
            if (equipmentStock == null) return NotFound();
            return Ok(equipmentStock);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] EquipmentStockDTO equipmentStockDto)
        {
            var added = await _equipmentStockService.AddAsync(equipmentStockDto);
            if (!added) return BadRequest("Failed to add EquipmentStock");
            return CreatedAtAction(nameof(GetById), new { id = equipmentStockDto.Id }, equipmentStockDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EquipmentStockDTO equipmentStockDto)
        {
            var updated = await _equipmentStockService.UpdateAsync(id, equipmentStockDto);
            if (!updated) return NotFound("EquipmentStock not found");

            return NoContent();
        }

        [HttpPut("updateAllEquipment/{equipmentStockId}")]
        public async Task<ActionResult<List<EquipmentDto>>> UpdateEquipmentsByEquipmentStockId(
    int equipmentStockId, [FromBody] EquipmentDto updatedEquipmentDto)
        {
            try
            {
                var updatedEquipment = _mapper.Map<Equipment>(updatedEquipmentDto);
                var updatedEquipments = await _equipmentStockService.UpdateEquipmentsByEquipmentStockIdAsync(equipmentStockId, updatedEquipment);

                var updatedEquipmentsDto = _mapper.Map<List<EquipmentDto>>(updatedEquipments);
                return Ok(updatedEquipmentsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-subequipment/{equipmentStockId}/{subEquipmentName}")]
        public async Task<IActionResult> UpdateSubEquipment(int equipmentStockId, string subEquipmentName, [FromBody] SubEquipment updatedSubEquipment)
        {
            var updated = await _equipmentStockService.UpdateSubEquipmentByNameAsync(equipmentStockId, subEquipmentName, updatedSubEquipment);
            if (!updated)
            {
                return NotFound("No matching SubEquipments found.");
            }
            return Ok("SubEquipments updated successfully.");
        }

        [HttpPost("{equipmentStockId}/subequipment")]
        public async Task<IActionResult> AddSubEquipment(int equipmentStockId, [FromBody] SubEquipment newSubEquipment)
        {
            if (newSubEquipment == null)
            {
                return BadRequest("Invalid sub-equipment data.");
            }

            var result = await _equipmentStockService.AddSubEquipmentToAllEquipmentsAsync(equipmentStockId, newSubEquipment);

            if (result)
            {
                return Ok("SubEquipment added to all Equipments under EquipmentStock.");
            }

            return NotFound("EquipmentStock not found or has no Equipments.");
        }

        [HttpDelete("subequipment")]
        public async Task<IActionResult> DeleteSubEquipmentFromAllEquipments(
       [FromQuery] int equipmentStockId,
       [FromQuery] string subEquipmentName)
        {
            var result = await _equipmentStockService.DeleteSubEquipmentFromAllEquipmentsAsync(equipmentStockId, subEquipmentName);

            if (!result)
                return NotFound("No matching sub-equipment found to delete.");

            return Ok("Sub-equipment deleted from all equipments.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _equipmentStockService.DeleteAsync(id);
            if (!deleted) return NotFound("EquipmentStock not found");

            return NoContent();
        }
    }
}
