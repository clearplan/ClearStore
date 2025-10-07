using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductColorCategory")]
    public partial class ProductColorCategory
    {
        [Key]
        [Column("ProductColorCategoryId")]
        public int Id { get; set; }

        public int Value { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = null!;
    }
}
