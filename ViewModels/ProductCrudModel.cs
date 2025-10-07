using ClearStore.Models;

#nullable disable
namespace ClearStore.ViewModels
{
    public class ProductCrudModel
    {
        public Product Product { get; set; }

        public List<ProductImage> Images { get; set; } = new();

        public List<IFormFile> ProductImages { get; set; } = new();

        public List<ProductColorCategory> ProductColorCategories { get; set; } = new();

        public List<ProductGender> ProductGenders { get; set; } = new();
    }
}
