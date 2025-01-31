using System.Collections.Generic;
using System.Threading.Tasks;

namespace sp_backend.Interfaces
{
    public interface IAccountService
    {
        Task<bool> CreateAccountAsync(Account account);
        Task<Account?> GetAccountByUsernameAsync(string username);
        Task<Account?> GetAccountByIdAsync(int id);
        Task<List<Account>> GetAllAccountsAsync();
        Task<bool> UpdateAccountAsync(int id, Account account);
        Task<bool> DeleteAccountAsync(int id); // Added DeleteAccountAsync
    }
}
