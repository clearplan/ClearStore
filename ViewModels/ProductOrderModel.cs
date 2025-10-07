using ClearStore.Models;
using ClearStore.Models.Dto;

#nullable disable
namespace ClearStore.ViewModels
{
    public class ProductOrderModel
    {
        public ProductOrder ProductOrder { get; set; }

        public ProductCart ProductCart { get; set; }

        public List<ProductDetailDto> OrderItems { get; set; } = new();

    }
}
