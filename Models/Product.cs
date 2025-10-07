using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("Product")]
    public partial class Product
    {
        [Key]
        [Column("ProductId")]
        public int Id { get; set; }

        [StringLength(256)]
        public string? Name { get; set; } = null!;

        public string? Description { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        public bool IsApparel { get; set; }

        public int? ProductGenderId { get; set; } = null!;

        [Column("ColorCategory")]
        public int? ProductColorCategoryId { get; set; } = null!;

        public bool IsVisible { get; set; }

        [ForeignKey(nameof(ProductGenderId))]
        public ProductGender? ProductGender { get; set; }

        [ForeignKey(nameof(ProductColorCategoryId))]
        public ProductColorCategory? ProductColorCategory { get; set; }

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        public ICollection<ProductInventory> ProductInventory { get; set; } = new List<ProductInventory>();
    }
}
