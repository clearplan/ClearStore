using ClearStore.Data;
using ClearStore.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClearStore.Controllers
{
    [AuthorizeForScopes(ScopeKeySection = "MicrosoftGraph:Scopes")]
    public class CartController : Controller
    {
        private readonly StoreContext _context;
        private readonly GraphServiceClient _client;

        public CartController(StoreContext context, GraphServiceClient client)
        {
            _context = context;
            _client = client;
        }


        public async Task<IActionResult> Index()
        {
            var user = (ClaimsPrincipal)User;
            var userId = user.FindFirst("uid")?.Value;

            var userCart = await _context.ProductCarts
                .Where(d => d.UserId == userId && d.Status == 1)
                .FirstOrDefaultAsync();

            List<ProductDetailDto> cart = new();

            if (userCart != null)
            {
                cart = await _context.ProductItems
                    .Where(pi => pi.ProductCartId == userCart.ProductCartId)
                    .Select(pi => new ProductDetailDto
                    {
                        ProductCartId = pi.ProductCartId,
                        ProductItemId = pi.ProductItemId,
                        ProductId = pi.ProductId,
                        ProductName = pi.Product != null ? pi.Product.Name : null,
                        ProductSizeId = pi.ProductSizeId,
                        SizeName = pi.ProductSize != null ? pi.ProductSize.Name : null,
                        ProductColorId = pi.ProductColorId,
                        ColorName = pi.ProductColor != null ? pi.ProductColor.Name : null,
                        Quantity = pi.Quantity,
                        ProductInventoryId = pi.ProductInventoryId,
                        Image = _context.ProductImages
                            .Where(d => d.ProductId == pi.ProductId)
                            .Select(i => i.ImageData)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                ViewBag.CartId = userCart.ProductCartId;
            }

            return View(cart);
        }
    }
}
