using ClearStore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace ClearStore.ViewComponents
{
    public class VCUserCart : ViewComponent
    {
        private readonly StoreContext _context;

        public VCUserCart(StoreContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = (ClaimsPrincipal)User;
            var userId = user.FindFirst("uid")?.Value;

            var count = await _context.ProductCarts
                .Where(c => c.UserId == userId && c.Status == 1)
                .SelectMany(c => c.ProductItems)
                .CountAsync();

            return View(count);
        }
    }
}
