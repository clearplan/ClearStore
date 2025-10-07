using ClearStore.Models;
using ClearStore.Models.Dto;

namespace ClearStore.ViewModels
{
    public class ProductOrderDetailModel
    {
        public ProductOrder ProductOrder { get; set; } = new();

        public List<ProductOrderItemDto> ProductOrderItems { get; set; } = new();
    }
}
