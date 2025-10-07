using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductApparel")]
    public partial class ProductApparel
    {
        [Key]
        [Column("ProductApparelId")]
        public int Id { get; set; }

        public int? ProductId { get; set; }

        public int? ProductSizeId { get; set; }

        public int? ProductColorId { get; set; }

        public int? ProductGenderId { get; set; }
    }
}
