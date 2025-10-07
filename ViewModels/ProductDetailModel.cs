using ClearStore.Models;

namespace ClearStore.ViewModels
{
    public class ProductDetailModel
    {
        public string UserId { get; set; } = null!;

        public int Quantity { get; set; }

        public int? SelectedColorId { get; set; }

        public int? SelectedSizeId { get; set; }

        public Product Product { get; set; } = new Product();

        public List<ProductColor> ProductColors { get; set; } = new List<ProductColor>();

        public List<ProductGender> ProductGenders { get; set; } = new List<ProductGender>();

        public List<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();

        public List<Product> SimilarProducts { get; set; } = new List<Product>();
    }
}
