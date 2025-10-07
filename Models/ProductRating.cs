using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ClearStore.Models
{
    [Table("ProductRating")]
    public partial class ProductRating
    {
        [Key]
        public int ProductRatingId { get; set; }
        public int Value { get; set; }
        public int ProductId { get; set; }
    }
}
