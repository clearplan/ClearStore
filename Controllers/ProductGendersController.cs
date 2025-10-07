using ClearStore.Data;
using ClearStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Controllers
{
    [Authorize(Policy = "storeadmins")]
    [Route("product-genders")]
    public class ProductGendersController : Controller
    {
        private readonly StoreContext _context;

        public ProductGendersController(StoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productGenders = await _context.ProductGenders
                .ToListAsync();

            return View(productGenders);
        }

        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var productGender = await _context.ProductGenders
                .FindAsync(id);

            if (productGender == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(productGender);
        }


        [Route("edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductGender productGender)
        {
            if (ModelState.IsValid)
            {
                _context.ProductGenders.Update(productGender);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productGender);
        }
    }
}
