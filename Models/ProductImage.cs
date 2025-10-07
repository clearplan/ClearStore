using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductImage")]
    public partial class ProductImage
    {
        [Key]
        [Column("ProductImageId")]  
        public int Id { get; set; }

        [StringLength(50)]
        public string? ImageName { get; set; }

        public int? ProductId { get; set; }

        public byte[]? ImageData { get; set; }

        [NotMapped]
        public bool Selected { get; set; }
    }
}
