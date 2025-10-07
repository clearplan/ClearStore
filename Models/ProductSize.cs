using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductSize")]
    public partial class ProductSize
    {
        [Key]
        [Column("ProductSizeId")]
        public int Id { get; set; }

        [StringLength(50)]
        public string? Name { get; set; } = null!;

        [NotMapped]
        public bool? SelectedSize { get; set; }

        [NotMapped]
        public int? SizeQuantity { get; set; }
    }
}
