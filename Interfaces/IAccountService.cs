using System.Collections.Generic;
using System.Threading.Tasks;
using sp_backend.DTO;

namespace sp_backend.Interfaces
{
    public interface IAccountService
    {
        Task<bool> CreateAccountAsync(Account account);
        Task<Account?> GetAccountByIdAsync(int id);
        Task<AccountDTO> GetByUsernameAsync(string username);
        Task<List<Account>> GetAllAccountsAsync();
        Task<bool> UpdateAccountAsync(int id, Account account);
        Task<bool> DeleteAccountAsync(int id); // Added DeleteAccountAsync
    }
}
