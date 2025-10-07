using ClearStore.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Controllers
{
    public class ImagesController : Controller
    {
        private readonly StoreContext _context;

        public ImagesController(StoreContext context)
        {
            _context = context;
        }

        public IActionResult GetImage(int id)
        {
            var image = _context.ProductImages
                .Where(i => i.Id == id)
                .Select(i => new { i.ImageData, i.ImageName })
                .FirstOrDefault();

            if (image == null || image.ImageData == null)
            {
                return NotFound();
            }

            var extension = Path.GetExtension(image.ImageName)?.ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            return File(image.ImageData, contentType);
        }
    }
}
