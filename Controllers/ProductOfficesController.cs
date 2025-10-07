using ClearStore.Data;
using ClearStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace ClearStore.Controllers
{
    [Authorize(Policy = "storeadmins")]
    [Route("product-offices")]
    public class ProductOfficesController : Controller
    {
        private readonly StoreContext _context;

        public ProductOfficesController(StoreContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var productOffices = _context.ProductOffices.ToList();
            return View(productOffices);
        }


        [Route("create")]
        public async Task<IActionResult> Create()
        {
            return View();
        }


        [Route("create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, ProductOffice productOffice)
        {
            if (ModelState.IsValid)
            {
                await _context.ProductOffices.AddAsync(productOffice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productOffice);
        }


        [Route("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var productOffice = await _context.ProductOffices.FindAsync(id);

            return View(productOffice);
        }


        [Route("edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductOffice productOffice)
        {
            if (ModelState.IsValid)
            {
                _context.ProductOffices.Update(productOffice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(productOffice);
        }
    }
}
