using ClearStore.Data;
using ClearStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Controllers
{
    [Authorize(Policy = "storeadmins")]
    [Route("product-colors")]
    public class ProductColorsController : Controller
    {
        private readonly StoreContext _context;

        public ProductColorsController(StoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productColors = await _context.ProductColors
                .Include(d => d.ProductColorCategory)
                .OrderBy(d => d.ProductColorCategoryId)
                .ToListAsync();

            return View(productColors);
        }

        [Route("create")]
        public async Task<IActionResult> Create()
        {
            var colorCategories = await _context.ProductColorCategories.AsNoTracking().ToListAsync();
            ViewBag.ColorCategories = colorCategories;

            return View();
        }


        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductColor productColor)
        {
            if (ModelState.IsValid)
            {
                await _context.ProductColors.AddAsync(productColor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var colorCategories = await _context.ProductColorCategories.AsNoTracking().ToListAsync();
            ViewBag.ColorCategories = colorCategories;

            return View(productColor);
        }


        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var productColor = await _context.ProductColors
                .Include(d => d.ProductColorCategory)
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();

            if (productColor == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var colorCategories = await _context.ProductColorCategories.AsNoTracking().ToListAsync();
            ViewBag.ColorCategories = colorCategories;

            return View(productColor);
        }


        [Route("edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductColor productColor)
        {
            if (ModelState.IsValid)
            {
                _context.ProductColors.Update(productColor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var colorCategories = await _context.ProductColorCategories.AsNoTracking().ToListAsync();
            ViewBag.ColorCategories = colorCategories;

            return View(productColor);
        }
    }
}
