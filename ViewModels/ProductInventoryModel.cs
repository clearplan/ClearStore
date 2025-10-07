using ClearStore.Models;
using ClearStore.Models.Dto;

#nullable disable
namespace ClearStore.ViewModels
{
    public class ProductInventoryModel
    {
        public List<ProductInventoryDto> ProductInventoryDto { get; set; } = new();

        public List<ProductSize> ProductSizes { get; set; } = new();

        public List<ProductColor> ProductColors { get; set; } = new();

        public List<ProductGender> ProductGenders { get; set; } = new();

        public List<ProductOffice> ProductOffices { get; set; } = new();
    }
}
