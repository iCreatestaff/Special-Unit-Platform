using WeatherApi.Interfaces;
using sp_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace WeatherApi.Services
{
    public class ItemService : IItemService
    {
        private readonly AppDbContext _context;

        public ItemService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await _context.Items.Include(i => i.Maintenance).ToListAsync();
        }

        public async Task<Item?> GetItemByIdAsync(int itemNumber)
        {
            return await _context.Items.Include(i => i.Maintenance)
                .FirstOrDefaultAsync(i => i.Item_number == itemNumber);
        }

        public async Task<Item> CreateItemAsync(Item item)
        {
            var maintenance = await _context.Maintenances.FindAsync(item.MaintenanceId);
            if (maintenance == null)
            {
                throw new Exception("Maintenance does not exist.");
            }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }


        public async Task<Item?> UpdateItemAsync(int itemNumber, Item item)
        {
            var existingItem = await _context.Items.FindAsync(itemNumber);
            if (existingItem == null)
            {
                return null;
            }

            existingItem.Type = item.Type ?? existingItem.Type;
            existingItem.Order_number = item.Order_number;
            existingItem.Product_group = item.Product_group;
            existingItem.Packing_quantity = item.Packing_quantity;
            existingItem.Packing_unit = item.Packing_unit;
            existingItem.End_of_repair = item.End_of_repair;
            existingItem.MaintenanceId = item.MaintenanceId;

            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<bool> DeleteItemAsync(int itemNumber)
        {
            var item = await _context.Items.FindAsync(itemNumber);
            if (item == null)
            {
                return false;
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
