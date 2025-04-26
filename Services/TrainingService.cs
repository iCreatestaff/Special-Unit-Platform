using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using sp_backend.Models;
using sp_backend_March4.DTO;
using sp_backend_March4.Interfaces;
using sp_backend_March4.Models;
using WeatherApi;

namespace sp_backend_March4.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TrainingService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TrainingDTO>> GetAllAsync()
        {
            var trainings = await _context.Trainings
                .Include(t => t.AccountTrainings)
                .ThenInclude(at => at.Account)
                .ToListAsync();
            return _mapper.Map<IEnumerable<TrainingDTO>>(trainings);
        }

        public async Task<TrainingDTO?> GetByIdAsync(int id)
        {
            var training = await _context.Trainings
                .Include(t => t.AccountTrainings)
                .ThenInclude(at => at.Account)
                .FirstOrDefaultAsync(t => t.Id == id);

            return training == null ? null : _mapper.Map<TrainingDTO>(training);
        }

        public async Task<bool> CreateAsync(TrainingDTO trainingDto)
        {
            if (trainingDto == null)
            {
                throw new ArgumentNullException(nameof(trainingDto));
            }

            var training = _mapper.Map<Training>(trainingDto);

            // Add the training first
            _context.Trainings.Add(training);
            await _context.SaveChangesAsync(); // Save to get ID

            // Ensure AccountTrainings is initialized
            training.AccountTrainings = new List<AccountTraining>();
            training.Status = "Pending";

            // Process each assigned account
            foreach (var accountId in trainingDto.AssignedAccounts.Distinct()) // Avoid duplicates
            {
                var accountTraining = new AccountTraining
                {
                    AccountId = accountId,
                    TrainingId = training.Id,
                    RegistrationDate = DateTime.UtcNow
                };

                training.AccountTrainings.Add(accountTraining);

                // Create NonAvailability for the account
                var nonAvailability = new Nonavailability
                {
                    AccountId = accountId,
                    Date1 = training.StartTime,
                    Date2 = training.EndTime,
                    Reason = $"Training: {training.Title}",
                    MissionID = training.Id
                };

                _context.Nonavailabilities.Add(nonAvailability);
            }


            // Save changes in a single transaction
            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<bool> UpdateAsync(int id, TrainingDTO trainingDto)
        {
            var existingTraining = await _context.Trainings.FindAsync(id);
            if (existingTraining == null) return false;

            var currentStatus = existingTraining.Status;

            _mapper.Map(trainingDto, existingTraining);

            existingTraining.Status = currentStatus; // Restore status

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var training = await _context.Trainings.FindAsync(id);
            if (training == null) return false;

            // 1. Delete related nonavailabilities (matching Reason with training title)
            var relatedNonavailabilities = await _context.Nonavailabilities
                .Where(n => n.Reason == $"Training: {training.Title}")
                .ToListAsync();

            _context.Nonavailabilities.RemoveRange(relatedNonavailabilities);

            // 2. Delete related notifications (matching by ReferenceId and Type)
            var relatedNotifications = await _context.Notifications
                .Where(n => n.ReferenceId == training.Id && n.Type == "training")
                .ToListAsync();

            _context.Notifications.RemoveRange(relatedNotifications);

            // 3. Delete the training itself
            _context.Trainings.Remove(training);

            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> AssignAccountToTraining(int trainingId, int accountId)
        {
            var training = await _context.Trainings.FindAsync(trainingId);
            var account = await _context.Accounts.FindAsync(accountId);
            if (training == null || account == null) return false;

            // Avoid duplicate entries
            bool alreadyAssigned = await _context.AccountTrainings
                .AnyAsync(at => at.TrainingId == trainingId && at.AccountId == accountId);
            if (alreadyAssigned) return false;

            var accountTraining = new AccountTraining
            {
                TrainingId = trainingId,
                AccountId = accountId
            };

            _context.AccountTrainings.Add(accountTraining);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<TrainingDTO>> GetTrainingsByAgentIdAsync(int agentId)
        {
            var trainings = await _context.Trainings
                .Where(t => t.AccountTrainings.Any(at => at.AccountId == agentId))  // Check if agent is linked
                .Include(t => t.AccountTrainings)  // Include related AccountTrainings
                .ToListAsync();

            var trainingDTOs = trainings.Select(t => new TrainingDTO
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                Location = t.Location,
                Status = t.Status,
                // Map AccountTrainings to AccountTrainingDTO
                AccountTrainings = t.AccountTrainings.Select(at => new AccountTraining
                {
                    AccountId = at.AccountId,
                    TrainingId = at.TrainingId
                }).ToList()
            }).ToList();

            return trainingDTOs;
        }

    }

}