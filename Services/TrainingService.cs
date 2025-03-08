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
            var training = _mapper.Map<Training>(trainingDto);
            _context.Trainings.Add(training);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(int id, TrainingDTO trainingDto)
        {
            var existingTraining = await _context.Trainings.FindAsync(id);
            if (existingTraining == null) return false;

            _mapper.Map(trainingDto, existingTraining);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var training = await _context.Trainings.FindAsync(id);
            if (training == null) return false;

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
    }

}