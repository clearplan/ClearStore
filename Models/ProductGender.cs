using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductGender")]
    public partial class ProductGender
    {
        [Key]
        [Column("ProductGenderId")]
        public int Id { get; set; }

        [StringLength(10)]
        public string? Name { get; set; } = null!;
    }
}
