using Microsoft.EntityFrameworkCore;
using sp_backend.DTO;
using sp_backend.Interfaces;
using sp_backend.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi;

public class AccountService : IAccountService
{
    private readonly AppDbContext _context;

    public AccountService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateAccountAsync(Account account)
    {
        // Prevent duplicate usernames
        if (await _context.Accounts.AnyAsync(a => a.Username == account.Username))
            return false;

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Account?> GetAccountByUsernameAsync(string username)
    {
        // Fetch account by username
        return await _context.Accounts.FirstOrDefaultAsync(a => a.Username == username);
    }

    public async Task<Account?> GetAccountByIdAsync(int id)
    {
        // Fetch account by ID
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<List<Account>> GetAllAccountsAsync()
    {
        // Fetch all accounts
        return await _context.Accounts.ToListAsync();
    }

    public async Task<bool> UpdateAccountAsync(int id, Account account)
    {
        // Find the account to be updated
        var existingAccount = await _context.Accounts.FindAsync(id);
        if (existingAccount == null)
        {
            return false; // Account not found
        }

        // Update properties (excluding ID, as it's not allowed to change)
        existingAccount.Username = account.Username;
        existingAccount.Name = account.Name;
        existingAccount.Role = account.Role;
        existingAccount.SocialFile = account.SocialFile;
        existingAccount.MedicalFile = account.MedicalFile;
        existingAccount.CareerFile = account.CareerFile;
        existingAccount.Nonavailabilities = account.Nonavailabilities;

        // Save changes to the database
        _context.Accounts.Update(existingAccount);
        await _context.SaveChangesAsync();

        return true; // Successful update
    }

    public async Task<bool> DeleteAccountAsync(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
            return false;

        _context.Accounts.Remove(account);
        return await _context.SaveChangesAsync() > 0;
    }

    // Fetch all missions assigned to a specific account
    public async Task<List<MissionDTO>> GetMissionsByAccountIdAsync(int accountId)
    {
        var account = await _context.Accounts
            .Include(a => a.Missions)  // Include the missions the account is assigned to
            .FirstOrDefaultAsync(a => a.Id == accountId);

        if (account == null) return new List<MissionDTO>();

        return account.Missions.Select(m => new MissionDTO
        {
            Id = m.Id,
            Description = m.Description,
            StartTime = m.StartTime,
            EndTime = m.EndTime,
            Location = m.Location,
            Status = m.Status,
            AdminId = m.AdminId,
            AssignedAccountIds = m.AssignedAccounts.Select(a => a.Id).ToList(),
            AssignedEquipmentIds = m.AssignedEquipment.Select(e => e.Id).ToList()
        }).ToList();
    }

    // Add an account to a mission (or update the association)
    public async Task<bool> AssignAccountToMissionAsync(int accountId, int missionId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        var mission = await _context.Missions.FindAsync(missionId);

        if (account == null || mission == null) return false;

        // Ensure the account isn't already assigned to this mission
        if (!mission.AssignedAccounts.Contains(account))
        {
            mission.AssignedAccounts.Add(account);
        }

        return await _context.SaveChangesAsync() > 0;
    }

    // Remove an account from a mission
    public async Task<bool> RemoveAccountFromMissionAsync(int accountId, int missionId)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        var mission = await _context.Missions.FindAsync(missionId);

        if (account == null || mission == null) return false;

        mission.AssignedAccounts.Remove(account);

        return await _context.SaveChangesAsync() > 0;
    }
}
