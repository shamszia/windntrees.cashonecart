using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("Order")]
    [Index(nameof(OrderNo), Name = "IX_Order_OrderNo", IsUnique = true)]
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime OrderTime { get; set; }
        public long OrderNo { get; set; }
        [StringLength(100)]
        public string OrderType { get; set; }
        [StringLength(100)]
        public string Title { get; set; }
        [Required]
        [StringLength(127)]
        public string Email { get; set; }
        [StringLength(15)]
        public string Cell { get; set; }
        [StringLength(100)]
        public string Company { get; set; }
        [Required]
        [StringLength(200)]
        public string Address { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        [StringLength(100)]
        public string Country { get; set; }
        [StringLength(15)]
        public string Status { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; }
        public byte[] RowVersion { get; set; }
        [StringLength(20)]
        public string SecretWord { get; set; }
        [StringLength(10)]
        public string PostalCode { get; set; }
        [StringLength(1000)]
        public string Remarks { get; set; }

        [InverseProperty(nameof(OrderItem.Order))]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
