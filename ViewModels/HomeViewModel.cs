using ClearStore.Models;
using ClearStore.Models.Dto;

namespace ClearStore.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<ProductDto> ProductDto { get; set; }

        public List<ProductGender> ProductGenders { get; set; } = new();
    }
}
