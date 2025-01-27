using Microsoft.AspNetCore.Mvc;  
using Microsoft.EntityFrameworkCore;  
using WeatherApi.Models;  
using System.Collections.Generic;  
using System.Linq;  
using System.Threading.Tasks;  

namespace WeatherApi.Controllers  
{  
    [ApiController]  
    [Route("api/[controller]")]  
    public class EquipmentController : ControllerBase  
    {  
        private readonly AppDbContext _context;  

        public EquipmentController(AppDbContext context)  
        {  
            _context = context;  
        }  

        // GET: api/equipment  
        [HttpGet]  
        public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipments()  
        {  
            return await _context.Equipments.Include(e => e.SubEquipments).ToListAsync();  
        }  

        // GET: api/equipment/{id}  
        [HttpGet("{id}")]  
        public async Task<ActionResult<Equipment>> GetEquipment(int id)  
        {  
            var equipment = await _context.Equipments.Include(e => e.SubEquipments)  
                .FirstOrDefaultAsync(e => e.Id == id);  
            if (equipment == null)  
            {  
                return NotFound();  
            }  

            return equipment;  
        }  

        // POST: api/equipment  
        [HttpPost]  
        public async Task<ActionResult<Equipment>> AddEquipment([FromBody] Equipment equipment)  
        {  
            _context.Equipments.Add(equipment);  
            await _context.SaveChangesAsync();  

            return CreatedAtAction(nameof(GetEquipment), new { id = equipment.Id }, equipment);  
        }  

        // PUT: api/equipment/{id}  
        [HttpPut("{id}")]  
        public async Task<IActionResult> UpdateEquipment(int id, [FromBody] Equipment equipment)  
        {  
            if (id != equipment.Id)  
            {  
                return BadRequest();  
            }  

            _context.Entry(equipment).State = EntityState.Modified;  

            try  
            {  
                await _context.SaveChangesAsync();  
            }  
            catch (DbUpdateConcurrencyException)  
            {  
                if (!_context.Equipments.Any(e => e.Id == id))  
                {  
                    return NotFound();  
                }  
                else  
                {  
                    throw;  
                }  
            }  

            return NoContent();  
        }  

        // DELETE: api/equipment/{id}  
        [HttpDelete("{id}")]  
        public async Task<IActionResult> DeleteEquipment(int id)  
        {  
            var equipment = await _context.Equipments.FindAsync(id);  
            if (equipment == null)  
            {  
                return NotFound();  
            }  

            _context.Equipments.Remove(equipment);  
            await _context.SaveChangesAsync();  

            return NoContent();  
        }  
    }  
}