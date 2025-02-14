using AutoMapper;
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
    private readonly IMapper _mapper;

    public AccountService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

    public async Task<Account?> GetAccountByIdAsync(int id)
    {
        // Fetch account by ID
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<List<Account>> GetAvailableAccountsAsync(DateTime d1, DateTime d2)
    {
        return await _context.Accounts
            .Where(a => !a.Nonavailabilities.Any(n =>
                (d1 >= n.Date1 && d1 <= n.Date2) ||  // d1 falls within a non-availability range
                (d2 >= n.Date1 && d2 <= n.Date2) ||  // d2 falls within a non-availability range
                (n.Date1 >= d1 && n.Date2 <= d2)     // non-availability is fully within the mission range
            ))
            .ToListAsync();
    }

    public async Task<List<AccountDTO>> GetAccountsByTypeAsync(string type)
    {
        return await _context.Accounts
            .Where(a => a.Type == type)
            .Select(a => new AccountDTO
            {
                Id = a.Id,
                Name = a.Name,
                Role = a.Role,
                Type = a.Type
            })
            .ToListAsync();
    }

    public async Task<List<AccountDTO>> GetAccountsByRoleAsync(string role)
    {
        return await _context.Accounts
            .Where(a => a.Role == role)  // Filter by role
            .Select(a => new AccountDTO
            {
                Id = a.Id,
                Name = a.Name,
                Role = a.Role,
                Photo = a.Photo // Assuming URL storage
            })
            .ToListAsync();
    }

    public async Task<AccountDTO> GetByUsernameAsync(string username)
    {
        var account = await _context.Accounts
            .FirstOrDefaultAsync(a => a.Username == username);

        if (account == null)
            return null;

        return _mapper.Map<AccountDTO>(account);
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
        existingAccount.Type = account.Type;
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

}