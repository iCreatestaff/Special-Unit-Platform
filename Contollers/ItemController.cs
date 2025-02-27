using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using sp_backend.DTO;
using sp_backend.Models;
using WeatherApi.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IMapper _mapper;

        public ItemController(IItemService itemService, IMapper mapper)
        {
            _itemService = itemService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemService.GetAllItemsAsync();
            return Ok(_mapper.Map<List<ItemDTO>>(items));
        }

        [HttpGet("{itemNumber}")]
        public async Task<IActionResult> GetItemById(int itemNumber)
        {
            var item = await _itemService.GetItemByIdAsync(itemNumber);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ItemDTO>(item));
        }

        [HttpPost]
        public async Task<IActionResult> CreateItem([FromBody] ItemDTO itemDTO)
        {
            if (itemDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            var item = _mapper.Map<Item>(itemDTO);
            var createdItem = await _itemService.CreateItemAsync(item);
            return CreatedAtAction(nameof(GetItemById), new { itemNumber = createdItem.Item_number }, _mapper.Map<ItemDTO>(createdItem));
        }

        [HttpPut("{itemNumber}")]
        public async Task<IActionResult> UpdateItem(int itemNumber, [FromBody] ItemDTO itemDTO)
        {
            var existingItem = await _itemService.GetItemByIdAsync(itemNumber);
            if (existingItem == null)
            {
                return NotFound();
            }

            var itemToUpdate = _mapper.Map<Item>(itemDTO);
            itemToUpdate.Item_number = itemNumber; // Ensure ItemNumber remains unchanged

            var updatedItem = await _itemService.UpdateItemAsync(itemNumber, itemToUpdate);
            return Ok(_mapper.Map<ItemDTO>(updatedItem));
        }

        [HttpDelete("{itemNumber}")]
        public async Task<IActionResult> DeleteItem(int itemNumber)
        {
            var deleted = await _itemService.DeleteItemAsync(itemNumber);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
