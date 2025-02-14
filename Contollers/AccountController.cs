using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sp_backend.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using sp_backend.Interfaces;
using sp_backend.Mappers;
using WeatherApi.Models;

[ApiController]
[Route("api/accounts")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IMissionService _missionService;  // Assuming you have a mission service

    public AccountController(IAccountService accountService, IMissionService missionService)
    {
        _accountService = accountService;
        _missionService = missionService;
    }

    // Ensure only SuperAdmin can create Admin accounts
    //[Authorize(Roles = "SuperAdmin")]
    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin([FromBody] AccountDTO dto)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var account = dto.ToEntity(passwordHash);
        account.Role = "Admin"; // Assign role as Admin

        var result = await _accountService.CreateAccountAsync(account);
        if (result)
            return Ok("Admin created successfully.");
        return BadRequest("Failed to create admin. username might already exist.");
    }

    // Ensure only Admin users can create Agent accounts
    //[Authorize(Roles = "Admin")]
    [HttpPost("create-agent")]
    public async Task<IActionResult> CreateAgent([FromBody] AccountDTO dto)
    {

        var passwordHash = "";
        var account = dto.ToEntity(passwordHash);
        account.Role = "Agent"; // Assign role as Agent

        var result = await _accountService.CreateAccountAsync(account);
        if (result)
            return Ok("Agent created successfully.");

        return BadRequest("Failed to create agent. Username might already exist.");
    }


    // [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<List<AccountDTO>>> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        var accountDtos = accounts.Select(a => a.ToDto()).ToList();
        return Ok(accountDtos);
    }

    [HttpGet("available")]
    public async Task<ActionResult<List<Account>>> GetAvailableAccounts(
    [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest("Start date must be before end date.");
        }

        var availableAccounts = await _accountService.GetAvailableAccountsAsync(startDate, endDate);

        if (!availableAccounts.Any())
        {
            return NotFound("No available accounts found for the given period.");
        }

        return Ok(availableAccounts);
    }

    [HttpGet("byType/{type}")]
    public async Task<IActionResult> GetAccountsByType(string type)
    {
        var accounts = await _accountService.GetAccountsByTypeAsync(type);
        return accounts.Any() ? Ok(accounts) : NotFound("No accounts found for the specified type.");
    }

    [HttpGet("by-role/{role}")]
    public async Task<IActionResult> GetAccountsByRole(string role)
    {
        var accounts = await _accountService.GetAccountsByRoleAsync(role);
        if (accounts == null || !accounts.Any())
        {
            return NotFound("No accounts found with the specified role.");
        }
        return Ok(accounts);
    }


    // [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountDTO>> GetAccountById(int id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);
        if (account != null)
        {
            return Ok(account.ToDto());
        }
        return NotFound("Account not found.");
    }
    [HttpGet("username/{username}")]
    public async Task<ActionResult<AccountDTO>> GetByUsername(string username)
    {
        var account = await _accountService.GetByUsernameAsync(username);

        if (account == null)
            return NotFound(new { message = "Account not found" });

        return Ok(account);
    }

    // [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAccount(int id, [FromBody] AccountDTO dto)
    {
        var existingAccount = await _accountService.GetAccountByIdAsync(id);
        if (existingAccount == null)
        {
            return NotFound("Account not found.");
        }

        // Update fields
        existingAccount.Username = dto.Username;
        existingAccount.Name = dto.Name;
        existingAccount.Type = dto.Type;
        existingAccount.Role = dto.Role;
        existingAccount.SocialFile = dto.SocialFile;
        existingAccount.MedicalFile = dto.MedicalFile;
        existingAccount.CareerFile = dto.CareerFile;
        existingAccount.Photo = dto.Photo;

        // Update password only if provided
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            existingAccount.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        }

        // Pass both parameters to match the method signature
        var result = await _accountService.UpdateAccountAsync(id, existingAccount);

        return result ? Ok("Account updated successfully.") : BadRequest("Failed to update account.");
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var result = await _accountService.DeleteAccountAsync(id);
        return result ? Ok("Account deleted successfully.") : NotFound("Account not found.");
    }
}
