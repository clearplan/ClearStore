using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductCart")]
    public partial class ProductCart
    {
        [Key]
        public int ProductCartId { get; set; }

        [StringLength(100)]
        public string? UserId { get; set; }

        [StringLength(36)]
        public string? CartGuid { get; set; }

        public int? Status { get; set; }

        public virtual ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
    }
}
