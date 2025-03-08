using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sp_backend_March4.DTO;
using sp_backend_March4.Interfaces;
using sp_backend_March4.Models;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController : ControllerBase
    {
        private readonly ITrainingService _trainingService;

        public TrainingController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trainings = await _trainingService.GetAllAsync();
            return Ok(trainings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var training = await _trainingService.GetByIdAsync(id);
            if (training == null) return NotFound();
            return Ok(training);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TrainingDTO trainingDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool created = await _trainingService.CreateAsync(trainingDto);
            if (!created) return BadRequest("Failed to create training.");
            return CreatedAtAction(nameof(GetById), new { id = trainingDto.Id }, trainingDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TrainingDTO trainingDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool updated = await _trainingService.UpdateAsync(id, trainingDto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool deleted = await _trainingService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("{trainingId}/assign/{accountId}")]
        public async Task<IActionResult> AssignAccountToTraining(int trainingId, int accountId)
        {
            bool assigned = await _trainingService.AssignAccountToTraining(trainingId, accountId);
            if (!assigned) return BadRequest("Failed to assign account to training.");
            return Ok("Account assigned successfully.");
        }
    }

}