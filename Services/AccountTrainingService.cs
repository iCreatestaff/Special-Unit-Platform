using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using sp_backend_March4.Interfaces;
using sp_backend_March4.Models;
using WeatherApi;

namespace sp_backend_March4.Services
{
    public class AccountTrainingService : IAccountTrainingService
    {
        private readonly AppDbContext _context;

        public AccountTrainingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AccountTraining> GetAccountTrainingAsync(int accountId, int trainingId)
        {
            return await _context.AccountTrainings
                .Include(at => at.Account)
                .Include(at => at.Training)
                .FirstOrDefaultAsync(at => at.AccountId == accountId && at.TrainingId == trainingId);
        }

        public async Task<IEnumerable<AccountTraining>> GetAllAccountTrainingsAsync()
        {
            return await _context.AccountTrainings
                .Include(at => at.Account)
                .Include(at => at.Training)
                .ToListAsync();
        }

        public async Task<AccountTraining> CreateAccountTrainingAsync(AccountTraining accountTraining)
        {
            // Make sure the accountTraining is not null
            if (accountTraining == null)
            {
                throw new ArgumentNullException(nameof(accountTraining));
            }

            // Set registration date
            accountTraining.RegistrationDate = DateTime.UtcNow;

            // Add the accountTraining to the context
            _context.AccountTrainings.Add(accountTraining);
            await _context.SaveChangesAsync();

            return accountTraining;
        }


        public async Task<AccountTraining> UpdateAccountTrainingAsync(int accountId, int trainingId, AccountTraining updatedAccountTraining)
        {
            var existingAccountTraining = await _context.AccountTrainings
                .FirstOrDefaultAsync(at => at.AccountId == accountId && at.TrainingId == trainingId);

            if (existingAccountTraining == null)
            {
                return null;
            }

            existingAccountTraining.RegistrationDate = updatedAccountTraining.RegistrationDate;

            _context.AccountTrainings.Update(existingAccountTraining);
            await _context.SaveChangesAsync();

            return existingAccountTraining;
        }

        public async Task<bool> DeleteAccountTrainingAsync(int accountId, int trainingId)
        {
            var accountTraining = await _context.AccountTrainings
                .FirstOrDefaultAsync(at => at.AccountId == accountId && at.TrainingId == trainingId);

            if (accountTraining == null)
            {
                return false;
            }

            _context.AccountTrainings.Remove(accountTraining);
            await _context.SaveChangesAsync();

            return true;
        }
    }

}