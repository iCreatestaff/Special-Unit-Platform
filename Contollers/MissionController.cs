using Microsoft.AspNetCore.Mvc;
using sp_backend.DTO;
using sp_backend.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sp_backend.Controllers
{
    [ApiController]
    [Route("api/missions")]
    public class MissionController : ControllerBase
    {
        private readonly IMissionService _missionService;
        private readonly IAccountService _accountService;

        public MissionController(IMissionService missionService, IAccountService accountService)
        {
            _missionService = missionService;
            _accountService = accountService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMission([FromBody] MissionDTO missionDTO)
        {

            if (missionDTO.StartTime > missionDTO.EndTime)
            {
                return BadRequest("Start Time must be before End Time.");
            }

            if (missionDTO == null)
            {
                return BadRequest("Mission data is required.");
            }

            // Validate AdminId
            var account = await _accountService.GetAccountByIdAsync(missionDTO.AdminId);
            if (account == null)
            {
                return BadRequest("Invalid AdminId: No Admin Found.");
            }
            if (account.Role != "Admin")
            {
                return BadRequest("Invalid AdminId: The user must be an Admin.");
            }

            // Create the mission
            var result = await _missionService.CreateMissionAsync(missionDTO);
            missionDTO.Status = "Pending";
            if (!result)
            {
                return BadRequest("Failed to create mission. Check if assigned accounts or equipment IDs are valid.");
            }

            return Ok("Mission created successfully.");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MissionDTO>> GetMission(int id)
        {
            var mission = await _missionService.GetMissionByIdAsync(id);
            return mission == null ? NotFound("Mission not found.") : Ok(mission);
        }

        [HttpGet("admin/{adminId}")]
        public async Task<IActionResult> GetMissionsByAdminId(int adminId)
        {
            var missions = await _missionService.GetMissionsByAdminIdAsync(adminId);

            if (missions == null || !missions.Any())
            {
                return NotFound($"No missions found for Admin ID {adminId}.");
            }

            return Ok(missions);
        }

        [HttpGet]
        public async Task<ActionResult<List<MissionDTO>>> GetAllMissions()
        {
            return Ok(await _missionService.GetAllMissionsAsync());
        }

        [HttpDelete("{missionId}/nonavailabilities")]
        public async Task<IActionResult> DeleteNonavailabilities(int missionId)
        {
            await _missionService.DeleteNonavailabilitiesByMissionId(missionId);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMission(int id, [FromBody] MissionDTO missionDTO)
        {
            var result = await _missionService.UpdateMissionAsync(id, missionDTO);
            return result ? Ok("Mission updated successfully") : NotFound("Mission not found.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMission(int id)
        {
            var result = await _missionService.DeleteMissionAsync(id);
            return result ? Ok("Mission deleted successfully") : NotFound("Mission not found.");
        }
    }
}
