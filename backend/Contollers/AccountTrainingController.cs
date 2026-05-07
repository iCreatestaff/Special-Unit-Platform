using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sp_backend_March4.Interfaces;
using sp_backend_March4.Models;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountTrainingController : ControllerBase
    {
        private readonly IAccountTrainingService _accountTrainingService;

        public AccountTrainingController(IAccountTrainingService accountTrainingService)
        {
            _accountTrainingService = accountTrainingService;
        }

        // GET: api/AccountTraining
        [HttpGet]  // Ensures that this method handles GET requests
        public async Task<ActionResult<IEnumerable<AccountTraining>>> GetAccountTrainings()
        {
            var accountTrainings = await _accountTrainingService.GetAllAccountTrainingsAsync();
            return Ok(accountTrainings);
        }

        // GET: api/AccountTraining/{accountId}/{trainingId}
        [HttpGet("{accountId}/{trainingId}")]  // This handles GET requests for a specific account and training
        public async Task<ActionResult<AccountTraining>> GetAccountTraining(int accountId, int trainingId)
        {
            var accountTraining = await _accountTrainingService.GetAccountTrainingAsync(accountId, trainingId);

            if (accountTraining == null)
            {
                return NotFound();
            }

            return Ok(accountTraining);
        }

        // POST: api/AccountTraining
        [HttpPost]  // Handles POST requests
        public async Task<ActionResult<AccountTraining>> CreateAccountTraining([FromBody] AccountTraining accountTraining)
        {
            if (accountTraining == null)
            {
                return BadRequest("AccountTraining data is null.");
            }

            var createdAccountTraining = await _accountTrainingService.CreateAccountTrainingAsync(accountTraining);

            // Return CreatedAtAction to respond with 201 status code
            return CreatedAtAction(nameof(GetAccountTraining), new { accountId = createdAccountTraining.AccountId, trainingId = createdAccountTraining.TrainingId }, createdAccountTraining);
        }

        // PUT: api/AccountTraining/{accountId}/{trainingId}
        [HttpPut("{accountId}/{trainingId}")]  // Handles PUT requests
        public async Task<IActionResult> UpdateAccountTraining(int accountId, int trainingId, [FromBody] AccountTraining updatedAccountTraining)
        {
            var accountTraining = await _accountTrainingService.UpdateAccountTrainingAsync(accountId, trainingId, updatedAccountTraining);

            if (accountTraining == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/AccountTraining/{accountId}/{trainingId}
        [HttpDelete("{accountId}/{trainingId}")]  // Handles DELETE requests
        public async Task<IActionResult> DeleteAccountTraining(int accountId, int trainingId)
        {
            var success = await _accountTrainingService.DeleteAccountTrainingAsync(accountId, trainingId);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
