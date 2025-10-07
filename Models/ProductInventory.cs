using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductInventory")]
    public partial class ProductInventory
    {
        [Key]
        [Column("ProductInventoryId")]
        public int Id { get; set; }

        public int? ProductId { get; set; }

        public int? ProductSizeId { get; set; }

        public int? ProductColorId { get; set; }

        public int? Quantity { get; set; }

        public int? ProductOfficeId { get; set; }

        public int? Threshold { get; set; }

        public bool? IsVisible { get; set; }

        [JsonIgnore]
        public Product? Product { get; set; }

        [JsonIgnore]
        public ProductSize? ProductSize { get; set; }

        [JsonIgnore]
        public ProductColor? ProductColor { get; set; }

        [JsonIgnore]
        public ProductOffice? ProductOffice { get; set; }

        [NotMapped]
        public bool? Selected { get; set; }
    }
}
