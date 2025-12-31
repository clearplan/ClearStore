namespace ClearStore.Models.Dto
{
    public class ProductOrderItemDto
    {
        public bool? IsApparel { get; set; }

        public int ProductItemId { get; set; }

        public int? ProductId { get; set; }

        public string? ProductName { get; set; }

        public int? ProductSizeId { get; set; }

        public string? SizeName { get; set; }

        public int? ProductColorId { get; set; }

        public string? ColorName { get; set; }

        public int? ProductGenderId { get; set; }

        public string? GenderName { get; set; }

        public int? Quantity { get; set; }

        public int? ProductCartId { get; set; }

        public int? ProductInventoryId { get; set; }

        //public byte[]? Image { get; set; }

        public ImageDetail? ImageDetail { get; set; }
    }
}
