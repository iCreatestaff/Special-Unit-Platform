using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sp_backend_March4.Models;

namespace sp_backend_March4.Interfaces
{
    public interface IAccountTrainingService
    {
        Task<AccountTraining> GetAccountTrainingAsync(int accountId, int trainingId);
        Task<IEnumerable<AccountTraining>> GetAllAccountTrainingsAsync();
        Task<AccountTraining> CreateAccountTrainingAsync(AccountTraining accountTraining);
        Task<AccountTraining> UpdateAccountTrainingAsync(int accountId, int trainingId, AccountTraining updatedAccountTraining);
        Task<bool> DeleteAccountTrainingAsync(int accountId, int trainingId);
    }

}