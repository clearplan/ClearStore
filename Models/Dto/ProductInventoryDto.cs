namespace ClearStore.Models.Dto
{
    public class ProductInventoryDto
    {
        public required Product Product { get; set; }

        public required List<ProductInventory> ProductInventory { get; set; } = new();

        public ProductImageDto? ProductImageDto { get; set; }
    }
}
