using ClearStore.Data;
using ClearStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Controllers
{
    [Authorize(Policy = "storeadmins")]
    [Route("product-sizes")]
    public class ProductSizesController : Controller
    {
        private readonly StoreContext _context;

        public ProductSizesController(StoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productSizes = await _context.ProductSizes
                .ToListAsync();

            return View(productSizes);
        }

        [Route("create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }


        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
                await _context.ProductSizes.AddAsync(productSize);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productSize);
        }

        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var productSize = await _context.ProductSizes
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();

            if (productSize == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(productSize);
        }


        [Route("edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductSize productSize)
        {
            if (ModelState.IsValid)
            {
                _context.ProductSizes.Update(productSize);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productSize);
        }
    }
}
