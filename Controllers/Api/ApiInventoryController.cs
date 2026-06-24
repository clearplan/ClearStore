using ClearStore.Data;
using ClearStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiInventoryController : ControllerBase
    {
        private readonly StoreContext _context;
        public ApiInventoryController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductInventory>> GetItem(int id)
        {
            var inventory = await _context.ProductInventory.FindAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            return inventory;
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateItem(int id, ProductInventory inventory)
        {
            if (id != inventory.Id)
            {
                return BadRequest("ID mismatch");
            }

            _context.Entry(inventory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ProductInventory.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }


        [HttpPost("save")]
        public async Task<ActionResult<ProductInventory>> SaveItem(ProductInventory inventory)
        {
            inventory.Id = 0;

            _context.ProductInventory.Add(inventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = inventory.Id }, inventory);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var inventory = await _context.ProductInventory.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            _context.ProductInventory.Remove(inventory);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
