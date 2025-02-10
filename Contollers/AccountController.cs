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
    public async Task<IActionResult> CreateAdmin([FromBody] AccountCreateDTO dto)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var account = dto.ToEntity(passwordHash);
        account.Role = "Admin"; // Assign role as Admin

        var result = await _accountService.CreateAccountAsync(account);
        if (result)
            return Ok("Admin created successfully.");
        return BadRequest("Failed to create admin. Email might already exist.");
    }

    // Ensure only Admin users can create Agent accounts
    //[Authorize(Roles = "Admin")]
    [HttpPost("create-agent")]
    public async Task<IActionResult> CreateAgent([FromBody] AccountCreateDTO dto)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var account = dto.ToEntity(passwordHash);
        account.Role = "Agent"; // Assign role as Agent

        var result = await _accountService.CreateAccountAsync(account);
        if (result)
            return Ok("Agent created successfully.");
        return BadRequest("Failed to create agent. Username might already exist.");
    }

    // [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpGet("all")]
    public async Task<ActionResult<List<AccountResponseDTO>>> GetAllAccounts()
    {
        var accounts = await _accountService.GetAllAccountsAsync();
        var accountDtos = accounts.Select(a => a.ToDto()).ToList();
        return Ok(accountDtos);
    }

    // [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<AccountResponseDTO>> GetAccountById(int id)
    {
        var account = await _accountService.GetAccountByIdAsync(id);
        if (account != null)
        {
            return Ok(account.ToDto());
        }
        return NotFound("Account not found.");
    }
    [HttpGet("username/{username}")]
    public async Task<ActionResult<AccountResponseDTO>> GetByUsername(string username)
    {
        var account = await _accountService.GetByUsernameAsync(username);

        if (account == null)
            return NotFound(new { message = "Account not found" });

        return Ok(account);
    }

    // [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateAccount(int id, [FromBody] AccountUpdateDTO dto)
    {
        var existingAccount = await _accountService.GetAccountByIdAsync(id);
        if (existingAccount == null)
        {
            return NotFound("Account not found.");
        }

        // Map DTO to entity and update the account
        var updatedAccount = dto.ToEntity();
        updatedAccount.Id = existingAccount.Id; // Ensure the Id remains unchanged

        var result = await _accountService.UpdateAccountAsync(id, updatedAccount);
        return result ? Ok("Account updated successfully.") : BadRequest("Failed to update account.");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAccount(int id)
    {
        var result = await _accountService.DeleteAccountAsync(id);
        return result ? Ok("Account deleted successfully.") : NotFound("Account not found.");
    }
}
