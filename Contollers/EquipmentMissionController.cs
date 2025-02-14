using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sp_backend.Models;
using sp_backend.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi;
using WeatherApi.DTOs;
using sp_backend.DTO.sp_backend.DTO;

namespace sp_backend.Controllers
{
    [Route("api/equipmentmission")]
    [ApiController]
    public class EquipmentMissionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EquipmentMissionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Get all equipment assigned to a specific mission
        [HttpGet("assigned-equipment/{missionId}")]
        public async Task<IActionResult> GetAssignedEquipment(int missionId)
        {
            var missionEquipment = await _context.EquipmentMissions
                .Where(em => em.MissionId == missionId)
                .Include(em => em.Equipment)
                .ToListAsync();

            if (!missionEquipment.Any())
            {
                return NotFound(new { Message = $"No equipment assigned to Mission ID {missionId}." });
            }

            var equipmentList = missionEquipment.Select(em => new EquipmentDto
            {
                Id = em.Equipment.Id,
                Name = em.Equipment.Name,
                Type = em.Equipment.Type
            }).ToList();

            return Ok(equipmentList);
        }

        // GET: Get all missions assigned to a specific equipment
        [HttpGet("assigned-missions/{equipmentId}")]
        public async Task<IActionResult> GetAssignedMissions(int equipmentId)
        {
            var equipmentMissions = await _context.EquipmentMissions
                .Where(em => em.EquipmentId == equipmentId)
                .Include(em => em.Mission)
                .ToListAsync();

            if (!equipmentMissions.Any())
            {
                return NotFound(new { Message = $"No missions assigned to Equipment ID {equipmentId}." });
            }

            var missionsList = equipmentMissions.Select(em => new MissionDTO
            {
                Id = em.Mission.Id,
                Type = em.Mission.Type,
                Description = em.Mission.Description,
                StartTime = em.Mission.StartTime,
                EndTime = em.Mission.EndTime,
                Location = em.Mission.Location,
                Status = em.Mission.Status,
                AdminId = em.Mission.AdminId

            }).ToList();

            return Ok(missionsList);
        }

        // POST: Assign equipment to a mission
        [HttpPost("assign")]
        public async Task<IActionResult> AssignEquipmentToMission([FromBody] EquipmentMissionDTO equipmentMissionDto)
        {
            if (equipmentMissionDto == null)
            {
                return BadRequest(new { Message = "Invalid data." });
            }

            var exists = await _context.EquipmentMissions.AnyAsync(em =>
                em.EquipmentId == equipmentMissionDto.EquipmentId &&
                em.MissionId == equipmentMissionDto.MissionId);

            if (exists)
            {
                return BadRequest(new { Message = "This equipment is already assigned to the mission." });
            }

            var equipmentMission = new EquipmentMission
            {
                EquipmentId = equipmentMissionDto.EquipmentId,
                MissionId = equipmentMissionDto.MissionId
            };

            _context.EquipmentMissions.Add(equipmentMission);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Equipment successfully assigned to mission." });
        }

        // DELETE: Remove equipment from a mission
        [HttpDelete("unassign/{equipmentId}/{missionId}")]
        public async Task<IActionResult> UnassignEquipmentFromMission(int equipmentId, int missionId)
        {
            var equipmentMission = await _context.EquipmentMissions
                .FirstOrDefaultAsync(em => em.EquipmentId == equipmentId && em.MissionId == missionId);

            if (equipmentMission == null)
            {
                return NotFound(new { Message = "Equipment assignment not found." });
            }

            _context.EquipmentMissions.Remove(equipmentMission);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Equipment successfully unassigned from mission." });
        }
    }
}
