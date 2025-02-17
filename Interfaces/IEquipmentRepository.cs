using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherApi.Models;

namespace sp_backend.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<Equipment> GetEquipmentByIdAsync(int id);
        Task<IEnumerable<Equipment>> GetAllEquipmentsAsync();
        Task<Equipment> CreateEquipmentAsync(Equipment equipment);
        Task<Equipment> UpdateEquipmentAsync(Equipment equipment);
        Task DeleteEquipmentAsync(int id);
    }

}