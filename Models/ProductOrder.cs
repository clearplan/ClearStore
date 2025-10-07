using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable
namespace ClearStore.Models
{
    [Table("ProductOrder")]
    public partial class ProductOrder
    {
        [Key]
        public int ProductOrderId { get; set; }

        public int? ProductCartId { get; set; }

        [StringLength(100)]
        public string Recipient { get; set; }

        [StringLength(256)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(3)]
        public string State { get; set; }

        [Display(Name = "Zip Code")]
        public int ZipCode { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public long? PhoneNumber { get; set; }

        public string Notes { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [Column("Status")]
        public int? StatusId { get; set; }

        [ForeignKey(nameof(StatusId))]
        public ProductOrderStatusCategory StatusCategory { get; set; }

    }
}
