using ClearStore.Data;
using ClearStore.Models;
using ClearStore.Models.Dto;
using ClearStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using System.Text;

namespace ClearStore.Controllers
{
    [Authorize(Policy = "storeadmins")]
    public class ProductInventoryController : Controller
    {
        private readonly StoreContext _context;

        public ProductInventoryController(StoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productInventory = _context.Products
                .AsNoTracking()
                .GroupJoin(_context.ProductInventory, p => p.Id, pi => pi.ProductId, (p, pi) => new { p, pi })
                .Select(s => new ProductInventoryDto
                {
                    Product = s.p,
                    ProductInventory = s.pi.ToList(),
                    ProductImageDto = _context.ProductImages
                        .Where(d => d.ProductId == s.p.Id)
                        .Select(x => new ProductImageDto
                        {
                            Id = x.Id,
                            ImageName = x.ImageName!
                        })
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.Product.Id)
                .ToList();

            var genders = await _context.ProductGenders.ToListAsync();
            var sizes = await _context.ProductSizes.ToListAsync();
            var colors = await _context.ProductColors.ToListAsync();
            var offices = await _context.ProductOffices.ToListAsync();

            var model = new ProductInventoryModel
            {
                ProductInventoryDto = productInventory,
                ProductSizes = sizes,
                ProductColors = colors,
                ProductGenders = genders,
                ProductOffices = offices
            };

            return View(model);
        }


        public async Task<IActionResult> Edit(int productId)
        {
            var model = await GetProductInventoryModelAsync(productId);

            return View(model);
        }


        private async Task<ProductInventoryCrudModel> GetProductInventoryModelAsync(int productId)
        {
            var productInventory = await _context.Products
                .Where(d => d.Id == productId)
                .AsNoTracking()
                .GroupJoin(
                    _context.ProductInventory,
                    p => p.Id,
                    pi => pi.ProductId,
                    (p, pi) => new { p, pi }
                )
                .Select(s => new ProductInventoryDto
                {
                    Product = s.p,
                    ProductInventory = s.pi.ToList(),
                    ProductImageDto = _context.ProductImages
                        .Where(d => d.ProductId == s.p.Id)
                        .Select(x => new ProductImageDto
                        {
                            Id = x.Id,
                            ImageName = x.ImageName!
                        })
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            var genders = await _context.ProductGenders.AsNoTracking().ToListAsync();
            var sizes = await _context.ProductSizes.AsNoTracking().ToListAsync();
            var colors = await _context.ProductColors.AsNoTracking().ToListAsync();
            var offices = await _context.ProductOffices.AsNoTracking().ToListAsync();

            return new ProductInventoryCrudModel
            {
                ProductInventoryDto = productInventory,
                ProductSizes = sizes,
                ProductColors = colors,
                ProductGenders = genders,
                ProductOffices = offices
            };
        }



        [HttpGet]
        public async Task<IActionResult> Export()
        {
            var query = await _context.Products
                .Include(d => d.ProductInventory)
                    .ThenInclude(d => d.ProductSize)
                .Include(d => d.ProductInventory)
                    .ThenInclude(d => d.ProductColor)
                .Include(d => d.ProductInventory)
                    .ThenInclude(d => d.ProductOffice)
                .ToListAsync();

            var inventory = query.SelectMany(d => d.ProductInventory).ToList();

            var sb = new StringBuilder();

            sb.AppendLine("Id,Product,Size,Color,Office,Quantity,IsVisible");

            foreach(var item in inventory)
            {
                var product = item.Product != null ? item.Product.Name : "N/A";
                var size = item.ProductSizeId != null && item.ProductSize != null ? item.ProductSize.Name : "N/A";
                var color = item.ProductColorId != null && item.ProductColor != null ? item.ProductColor.Name : "N/A";
                var office = item.ProductOfficeId != null && item.ProductOffice != null ? item.ProductOffice.Location : "N/A";
                var visibility = item.IsVisible == true ? "Visible" : "Hidden";

                string row = $"{item.Id},{product},{size},{color},{office},{item.Quantity},{visibility}";
                
                sb.AppendLine(row);
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            return File(bytes, "text/csv", "store-inventory.csv");
        }

    }
}
