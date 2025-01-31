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

        public MissionController(IMissionService missionService)
        {
            _missionService = missionService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateMission([FromBody] MissionDTO missionDTO)
        {
            var result = await _missionService.CreateMissionAsync(missionDTO);
            return result ? Ok("Mission created successfully") : BadRequest("Failed to create mission.");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MissionDTO>> GetMission(int id)
        {
            var mission = await _missionService.GetMissionByIdAsync(id);
            return mission == null ? NotFound("Mission not found.") : Ok(mission);
        }

        [HttpGet]
        public async Task<ActionResult<List<MissionDTO>>> GetAllMissions()
        {
            return Ok(await _missionService.GetAllMissionsAsync());
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
