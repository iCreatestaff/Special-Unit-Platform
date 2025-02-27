using sp_backend.Models;

namespace WeatherApi.Interfaces
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task<Item?> GetItemByIdAsync(int itemNumber);
        Task<Item> CreateItemAsync(Item item);
        Task<Item?> UpdateItemAsync(int itemNumber, Item item);
        Task<bool> DeleteItemAsync(int itemNumber);
    }
}
