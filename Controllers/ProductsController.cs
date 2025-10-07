using ClearStore.Data;
using ClearStore.Models;
using ClearStore.Models.Dto;
using ClearStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ClearStore.Controllers
{
    [Authorize(Policy = "storeadmins")]
    public class ProductsController : Controller
    {
        private readonly StoreContext _context;

        public ProductsController(StoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var productQuery = _context.Products
                .AsNoTracking()
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

            var products = await productQuery
                .OrderByDescending(d => d.Product.ModifiedDate)
                .ToListAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            var colorCategories = _context.ProductColorCategories.ToList();
            var genders = _context.ProductGenders.ToList();

            var model = new ProductCrudModel
            {
                ProductColorCategories = colorCategories,
                ProductGenders = genders
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCrudModel model)
        {
            using var transation = await _context.Database.BeginTransactionAsync();
            var productImages = new List<ProductImage>();

            try
            {
                if (ModelState.IsValid)
                {
                    await _context.Products.AddAsync(model.Product);
                    await _context.SaveChangesAsync();

                    if (model.ProductImages != null)
                    {
                        foreach (var image in model.ProductImages)
                        {
                            if (image.Length > 0)
                            {
                                string path = Path.GetFileName(image?.FileName)!;
                                string fileExtension = Path.GetExtension(path);
                                string imageName = string.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                                var productImage = new ProductImage()
                                {
                                    ImageName = imageName,
                                    ProductId = model.Product.Id
                                };

                                using (var stream = new MemoryStream())
                                {
                                    await image!.CopyToAsync(stream);
                                    productImage.ImageData = stream.ToArray();
                                }

                                productImages.Add(productImage);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "The uploaded image does not contain identifiable data. Please upload a valid image.");
                            }
                        }

                        if (productImages != null)
                        {
                            await _context.ProductImages.AddRangeAsync(productImages);
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transation.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"{ex.Message}");
                return View(model);
            }

            var colorCategories = _context.ProductColorCategories.ToList();
            var genders = _context.ProductGenders.ToList();

            model.ProductColorCategories = colorCategories;
            model.ProductGenders = genders;

            return View(model);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var colorCategories = _context.ProductColorCategories.ToList();
            var genders = _context.ProductGenders.ToList();
            var images = _context.ProductImages.Where(d => d.ProductId == product.Id).ToList();

            var model = new ProductCrudModel
            {
                Product = product,
                ProductColorCategories = colorCategories,
                ProductGenders = genders,
                Images = images
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCrudModel model)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var productImages = new List<ProductImage>();

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Products.Update(model.Product);

                    // add new product images - don't update existing ones
                    if (model.ProductImages != null)
                    {
                        foreach (var image in model.ProductImages)
                        {
                            if (image.Length > 0)
                            {
                                string path = Path.GetFileName(image?.FileName)!;
                                string fileExtension = Path.GetExtension(path);
                                string imageName = string.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);

                                var productImage = new ProductImage()
                                {
                                    ImageName = imageName,
                                    ProductId = model.Product.Id
                                };

                                using (var stream = new MemoryStream())
                                {
                                    await image!.CopyToAsync(stream);
                                    productImage.ImageData = stream.ToArray();
                                }

                                productImages.Add(productImage);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "The uploaded image does not contain identifiable data. Please upload a valid image.");
                            }
                        }

                        if (productImages != null)
                        {
                            await _context.ProductImages.AddRangeAsync(productImages);
                        }
                    }

                    await _context.SaveChangesAsync();  
                    await transaction.CommitAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"An error occurred updating this product: {ex.Message}");
                return View(model);
            }

            var colorCategories = _context.ProductColorCategories.ToList();
            var genders = _context.ProductGenders.ToList();
            var images = _context.ProductImages.Where(d => d.ProductId == model.Product.Id).ToList();

            model.ProductColorCategories = colorCategories;
            model.ProductGenders = genders;
            model.Images = images;

            return View(model);
        }
    }
}
