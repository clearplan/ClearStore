using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductOffice")]
    public partial class ProductOffice
    {
        [Key]
        [Column("ProductOfficeId")]
        public int Id { get; set; }

        [StringLength(100)]
        public string? Location { get; set; } = null!;

        public bool CanShip { get; set; }
    }
}
