namespace ClearStore.Models.Dto
{
    public class ProductDto
    {
        public required Product Product { get; set; }

        public ProductImageDto? ProductImageDto { get; set; }
    }
}
