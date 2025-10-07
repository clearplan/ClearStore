using ClearStore.Models;
using ClearStore.Models.Dto;

#nullable disable
namespace ClearStore.ViewModels
{
    public class ProductInventoryCrudModel
    {
        public ProductInventoryDto ProductInventoryDto { get; set; }

        public List<ProductSize> ProductSizes { get; set; } = new();

        public List<ProductColor> ProductColors { get; set; } = new();

        public List<ProductGender> ProductGenders { get; set; } = new();

        public List<ProductOffice> ProductOffices { get; set; } = new();
    }
}
