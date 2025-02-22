using Microsoft.AspNetCore.Mvc;
using sp_backend.DTO;
using sp_backend.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sp_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipmentStockController : ControllerBase
    {
        private readonly IEquipmentStockService _equipmentStockService;

        public EquipmentStockController(IEquipmentStockService equipmentStockService)
        {
            _equipmentStockService = equipmentStockService;
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _equipmentStockService.DeleteAsync(id);
            if (!deleted) return NotFound("EquipmentStock not found");

            return NoContent();
        }
    }
}
