using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductCategory")]
    public partial class ProductCategory
    {
        [Key]
        public int ProductCategoryId { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public int ProductId { get; set; }
    }
}
