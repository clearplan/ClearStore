using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductColor")]
    public partial class ProductColor
    {
        [Key]
        [Column("ProductColorID")]  
        public int Id { get; set; }

        [StringLength(20)]
        public string? Name { get; set; } = null!;

        [Display(Name = "Color Category")]
        [Column("ColorCategory")]
        public int? ProductColorCategoryId { get; set; }

        [NotMapped]
        public bool? SelectedColor { get; set; }

        public virtual ProductColorCategory? ProductColorCategory { get; set; }
    }
}
