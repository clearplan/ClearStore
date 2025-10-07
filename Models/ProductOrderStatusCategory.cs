using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClearStore.Models
{
    [Table("ProductOrderStatusCategory")]
    public class ProductOrderStatusCategory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;

    }
}
