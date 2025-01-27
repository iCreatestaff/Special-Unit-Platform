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
    public class SubEquipmentController : ControllerBase  
    {  
        private readonly AppDbContext _context;  

        public SubEquipmentController(AppDbContext context)  
        {  
            _context = context;  
        }  

        // GET: api/subequipment  
        [HttpGet]  
        public async Task<ActionResult<IEnumerable<SubEquipment>>> GetSubEquipments()  
        {  
            return await _context.SubEquipments.Include(se => se.Equipment).ToListAsync(); // Optionally include Equipment  
        }  

        // GET: api/subequipment/{id}  
        [HttpGet("{id}")]  
        public async Task<ActionResult<SubEquipment>> GetSubEquipment(int id)  
        {  
            var subEquipment = await _context.SubEquipments.FindAsync(id);  
            if (subEquipment == null)  
            {  
                return NotFound();  
            }  

            return subEquipment;  
        }  

        // POST: api/subequipment  
        [HttpPost]  
        public async Task<ActionResult<SubEquipment>> AddSubEquipment([FromBody] SubEquipment subEquipment)  
        {  
            var equipment = await _context.Equipments.FindAsync(subEquipment.EquipmentId);  
            if (equipment == null)  
            {  
                return NotFound("Associated equipment not found.");  
            }  

            _context.SubEquipments.Add(subEquipment);  
            await _context.SaveChangesAsync();  

            return CreatedAtAction(nameof(GetSubEquipment), new { id = subEquipment.Id }, subEquipment);  
        }  

        // PUT: api/subequipment/{id}  
        [HttpPut("{id}")]  
        public async Task<IActionResult> UpdateSubEquipment(int id, [FromBody] SubEquipment subEquipment)  
        {  
            if (id != subEquipment.Id)  
            {  
                return BadRequest();  
            }  

            _context.Entry(subEquipment).State = EntityState.Modified;  

            try  
            {  
                await _context.SaveChangesAsync();  
            }  
            catch (DbUpdateConcurrencyException)  
            {  
                if (!_context.SubEquipments.Any(se => se.Id == id))  
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

        // DELETE: api/subequipment/{id}  
        [HttpDelete("{id}")]  
        public async Task<IActionResult> DeleteSubEquipment(int id)  
        {  
            var subEquipment = await _context.SubEquipments.FindAsync(id);  
            if (subEquipment == null)  
            {  
                return NotFound();  
            }  

            _context.SubEquipments.Remove(subEquipment);  
            await _context.SaveChangesAsync();  

            return NoContent();  
        }  
    }  
}