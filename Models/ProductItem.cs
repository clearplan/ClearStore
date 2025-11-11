using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductItem")]
    public partial class ProductItem
    {
        [Key]
        public int ProductItemId { get; set; }

        public int? ProductId { get; set; }

        public int? ProductSizeId { get; set; }

        public int? ProductColorId { get; set; }

        public int? ProductGenderId { get; set; }

        public string? UserId { get; set; }

        public int? Quantity { get; set; }

        public int? ProductCartId { get; set; }

        public int? ProductInventoryId { get; set; }


        [ForeignKey(nameof(ProductId))]
        public virtual Product? Product { get; set; }

        [ForeignKey(nameof(ProductSizeId))]
        public virtual ProductSize? ProductSize { get; set; }

        [ForeignKey(nameof(ProductColorId))]
        public virtual ProductColor? ProductColor { get; set; }
        
        //[ForeignKey(nameof(ProductGenderId))]
        //public virtual ProductGender? ProductGender { get; set; }

        [ForeignKey(nameof(ProductCartId))]
        public virtual ProductCart? ProductCart { get; set; }

        [ForeignKey(nameof(ProductInventoryId))]
        public virtual ProductInventory? ProductInventory { get; set; }

        [NotMapped]
        public bool? SelectedColor { get; set; }

        [NotMapped]
        public bool? SelectedSize { get; set; }

    }
}
