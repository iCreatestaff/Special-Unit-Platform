using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sp_backend.DTO;
using sp_backend.Models;
using WeatherApi;

namespace sp_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountMissionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountMissionController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/accountmission
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountMissionDTO>>> GetAccountMissions()
        {
            var accountMissions = await _context.AccountMissions
                .Select(am => new AccountMissionDTO
                {
                    MissionId = am.MissionId,
                    AccountId = am.AccountId,
                    AssignedDate = am.AssignedDate
                })
                .ToListAsync();

            return Ok(accountMissions);
        }

        // GET: api/accountmission/{accountId}/{missionId}
        [HttpGet("{accountId}/{missionId}")]
        public async Task<ActionResult<AccountMissionDTO>> GetAccountMission(int accountId, int missionId)
        {
            var accountMission = await _context.AccountMissions
                .Where(am => am.AccountId == accountId && am.MissionId == missionId)
                .Select(am => new AccountMissionDTO
                {
                    MissionId = am.MissionId,
                    AccountId = am.AccountId,
                    AssignedDate = am.AssignedDate
                })
                .FirstOrDefaultAsync();

            if (accountMission == null)
            {
                return NotFound();
            }

            return Ok(accountMission);
        }
        [HttpGet("assigned-missions/{accountId}")]
        public async Task<IActionResult> GetAssignedMissions(int accountId)
        {
            var accountMissions = await _context.AccountMissions
                .Where(am => am.AccountId == accountId)
                .Include(am => am.Mission)  // Include mission details
                .ToListAsync();

            if (!accountMissions.Any())
            {
                return NotFound($"No missions found for Account ID {accountId}.");
            }

            var missions = accountMissions.Select(am => new
            {
                am.Mission.Id,
                am.Mission.Description,
                am.Mission.StartTime,
                am.Mission.EndTime,
                am.Mission.Location,
                am.Mission.Status
            });

            return Ok(missions);
        }

        [HttpGet("assigned-accounts/{missionId}")]
        public async Task<IActionResult> GetAssignedAccounts(int missionId)
        {
            var missionAccounts = await _context.AccountMissions
                .Where(am => am.MissionId == missionId)
                .Include(am => am.Account)  // Include account details
                .ToListAsync();

            if (!missionAccounts.Any())
            {
                return NotFound($"No accounts assigned to Mission ID {missionId}.");
            }

            var accounts = missionAccounts.Select(am => new
            {
                am.Account.Id,
                am.Account.Name,
                am.Account.Username,
                am.Account.Role
            });

            return Ok(accounts);
        }


        // POST: api/accountmission
        [HttpPost]
        public async Task<ActionResult<AccountMission>> AddAccountMission(AccountMissionDTO accountMissionDto)
        {
            var accountMission = new AccountMission
            {
                MissionId = accountMissionDto.MissionId,
                AccountId = accountMissionDto.AccountId,
                AssignedDate = accountMissionDto.AssignedDate
            };

            _context.AccountMissions.Add(accountMission);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAccountMission), new { accountId = accountMission.AccountId, missionId = accountMission.MissionId }, accountMissionDto);
        }

        // DELETE: api/accountmission/{accountId}/{missionId}
        [HttpDelete("{accountId}/{missionId}")]
        public async Task<IActionResult> DeleteAccountMission(int accountId, int missionId)
        {
            var accountMission = await _context.AccountMissions
                .FirstOrDefaultAsync(am => am.AccountId == accountId && am.MissionId == missionId);

            if (accountMission == null)
            {
                return NotFound();
            }

            _context.AccountMissions.Remove(accountMission);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
