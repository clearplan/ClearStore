using ClearStore.Data;
using ClearStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Threading.Tasks;

namespace ClearStore.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiProductsController : ControllerBase
    {
        private readonly StoreContext _context;

        public ApiProductsController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products
                .Where(d => d.Id == id)
                .Include(d => d.ProductGender)
                .Include(d => d.ProductColorCategory)
                .Include(d => d.ProductImages)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("product/{productId}/inventory")]
        public async Task<IActionResult> GetInventoryAsync(int productId, [FromQuery] int? colorId, [FromQuery] int? sizeId)
        {
            var query = _context.ProductInventory
                .Where(pi => pi.ProductId == productId && pi.IsVisible == true);

            if (colorId.HasValue)
            {
                query = query.Where(pi => pi.ProductColorId == colorId.Value);
            }

            if (sizeId.HasValue)
            {
                query = query.Where(pi => pi.ProductSizeId == sizeId.Value);
            }

            var results = await query.Select(pi => new
            {
                inventoryId = pi.Id,
                productId = pi.ProductId,
                colorId = pi.ProductColorId,
                sizeId = pi.ProductSizeId,
                quantity = pi.Quantity ?? 0
            }).ToListAsync();

            return Ok(results);
        }



        [HttpGet("inventory/item/update/{id}")]
        public async Task<IActionResult> UpdateInventoryAsync(int id, [FromQuery] int quantity)
        {
            var inventory = await _context.ProductInventory.FindAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            inventory.Quantity = quantity;

            _context.ProductInventory.Update(inventory);
            await _context.SaveChangesAsync();

            return Ok(inventory);
        }



        [HttpPut("product/update-visibility/{id}")]
        public async Task<IActionResult> UpdateProductVisibility(int id, [FromBody]bool isVisible)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // update the Visibility column
            product.IsVisible = isVisible;

            _context.Entry(product).Property(p => p.IsVisible).IsModified = true;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true
            });
        }



        [HttpDelete("product/delete/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true
            });
        }


        [HttpDelete("cart/delete/{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _context.ProductItems.FindAsync(id);
            var cartItems = await _context.ProductItems.Where(d => d.ProductCartId != id).CountAsync();

            if (cartItem == null)
            {
                return NotFound();
            }

            _context.ProductItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                totalCartItems = cartItems
            });
        }



        [HttpDelete("orders/delete/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.ProductOrders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            _context.ProductOrders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true
            });
        }



        [HttpDelete("order-item/delete/{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var orderItem = await _context.ProductItems.FindAsync(id);
            var orderItems = await _context.ProductItems.Where(d => d.ProductItemId != id).CountAsync();

            if (orderItem == null)
            {
                return NotFound();
            }

            _context.ProductItems.Remove(orderItem);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                totalOrderItems = orderItems
            });
        }


        [HttpDelete("image/delete/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);

            if (productImage == null)
            {
                return NotFound();
            }

            _context.ProductImages.Remove(productImage);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true
            });
        }



        [HttpDelete("inventory/delete/{id}")]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            var inventory = await _context.ProductInventory.FindAsync(id);

            if (inventory == null)
            {
                return NotFound();
            }

            _context.ProductInventory.Remove(inventory);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true
            });
        }
    }
}
