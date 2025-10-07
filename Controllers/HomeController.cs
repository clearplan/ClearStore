using ClearStore.Data;
using ClearStore.Models;
using ClearStore.Models.Dto;
using ClearStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Diagnostics;
using System.Security.Claims;

namespace ClearStore.Controllers
{

    public class HomeController : Controller
    {
        private readonly StoreContext _context;

        public HomeController(StoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(bool? isApparel = null)
        {
            var productQuery = _context.Products
                .Where(p => p.IsVisible == true)
                .Include(d => d.ProductGender)
                .AsNoTracking()
                .Where(p => _context.ProductInventory
                    .Any(pi => pi.ProductId == p.Id && pi.Quantity > 0))
                .Select(p => new ProductDto
                {
                    Product = p,
                    ProductImageDto = _context.ProductImages
                        .Where(img => img.ProductId == p.Id)
                        .Select(img => new ProductImageDto
                        {
                            Id = img.Id,
                            ImageName = img.ImageName!
                        })
                        .FirstOrDefault()
                })
                .AsQueryable();

            if (isApparel.HasValue)
            {
                productQuery = productQuery.Where(d => d.Product.IsApparel == isApparel.Value);
            }

            var products = await productQuery
                .OrderByDescending(d => d.Product.ModifiedDate)
                .ToListAsync();

            ViewBag.OrderComplete = TempData["OrderComplete"] as bool?;
            ViewBag.CartCountError = TempData["CartCountError"] as bool?;
            ViewBag.ItemAdded = TempData["ItemAdded"] as bool?;
            ViewBag.IsApparel = isApparel;

            var productGenders = await _context.ProductGenders
                .AsNoTracking()
                .ToListAsync();

            var model = new HomeViewModel
            {
                ProductDto = products,
                ProductGenders = productGenders
            };

            return View(model);
        }


        [Route("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var user = (ClaimsPrincipal)User;
            var userId = user.FindFirst("uid")?.Value;

            var model = await BuildProductDetailModelAsync(id);

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("details/{id}")]
        public async Task<IActionResult> Details(int id, ProductDetailModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (ModelState.IsValid)
                {
                    var cart = _context.ProductCarts.Where(d => d.UserId == model.UserId && d.Status == 1).FirstOrDefault();

                    var productItem = new ProductItem
                    {
                        ProductId = model.Product.Id,
                        UserId = model.UserId,
                        Quantity = model.Quantity
                    };

                    if (cart != null)
                    {
                        productItem.ProductCartId = cart.ProductCartId;
                    }
                    else
                    {
                        var newCart = new ProductCart
                        {
                            UserId = model.UserId,
                            Status = 1,
                            CartGuid = Guid.NewGuid().ToString()
                        };

                        await _context.ProductCarts.AddAsync(newCart);
                        await _context.SaveChangesAsync();

                        productItem.ProductCartId = newCart.ProductCartId;
                    }

                    int? sizeId = model.SelectedSizeId;
                    int? colorId = model.SelectedColorId;

                    var productInventory = _context.ProductInventory.Where(d => d.ProductId == model.Product.Id).AsQueryable();

                    if (colorId != null)
                    {
                        productItem.ProductColorId = colorId;
                        productInventory = productInventory.Where(d => d.ProductColorId == colorId);
                    }
                    if (sizeId != null)
                    {
                        productItem.ProductSizeId = sizeId;
                        productInventory = productInventory.Where(d => d.ProductSizeId == sizeId);
                    }

                    var inventoryItem = productInventory.FirstOrDefault();

                    if (inventoryItem != null)
                    {
                        productItem.ProductInventoryId = inventoryItem.Id;
                        inventoryItem.Quantity -= productItem.Quantity;
                    }

                    await _context.ProductItems.AddAsync(productItem);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["ItemAdded"] = true;
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception error)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, error.Message);
                return View(model);
            }

            model = await BuildProductDetailModelAsync(id);

            return View(model);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        private async Task<ProductDetailModel> BuildProductDetailModelAsync(int id)
        {
            var userId = User.FindFirst("uid")?.Value;

            var model = new ProductDetailModel
            {
                UserId = userId ?? string.Empty
            };

            var product = await _context.Products
                .Where(d => d.Id == id)
                .Include(d => d.ProductGender)
                .Include(d => d.ProductImages)
                .Include(d => d.ProductInventory)
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return model;
            }

            var productGenders = await _context.ProductGenders.ToListAsync();

            var similarProducts = await _context.Products
                .Where(d => d.IsApparel == product.IsApparel
                            && d.Id != product.Id
                            && d.IsVisible
                            && d.ProductGenderId == product.ProductGenderId)
                .Include(d => d.ProductImages)
                .OrderByDescending(d => d.ModifiedDate)
                .Take(4)
                .ToListAsync();

            var inventory = _context.ProductInventory.Where(d => d.ProductId == id);

            if (product.IsApparel)
            {
                var availableColors = _context.ProductColors
                    .Join(inventory, c => c.Id, i => i.ProductColorId, (c, i) => new { c, i })
                    .GroupBy(g => new { g.c.Id, g.c.Name })
                    .Select(s => new ProductColor
                    {
                        Id = s.Key.Id,
                        Name = s.Key.Name
                    }).ToList();

                model.ProductColors = availableColors;

                var availableSizes = _context.ProductSizes
                    .Join(inventory, s => s.Id, i => i.ProductSizeId, (s, i) => new { s, i })
                    .GroupBy(g => new { g.s.Id, g.s.Name })
                    .Select(x => new ProductSize
                    {
                        Id = x.Key.Id,
                        Name = x.Key.Name,
                        SizeQuantity = inventory.Select(v => v.Quantity).FirstOrDefault()
                    }).ToList();

                model.ProductSizes = availableSizes;
            }

            model.Product = product;
            model.ProductGenders = productGenders;
            model.SimilarProducts = similarProducts;

            return model;
        }
    }
}
