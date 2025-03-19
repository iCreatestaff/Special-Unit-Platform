using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sp_backend_March4.DTO;

namespace sp_backend_March4.Interfaces
{
    public interface ITrainingService
    {
        Task<IEnumerable<TrainingDTO>> GetAllAsync();
        Task<TrainingDTO?> GetByIdAsync(int id);
        Task<bool> CreateAsync(TrainingDTO trainingDto);
        Task<List<TrainingDTO>> GetTrainingsByAgentIdAsync(int agentId);
        Task<bool> UpdateAsync(int id, TrainingDTO trainingDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AssignAccountToTraining(int trainingId, int accountId);
    }

}