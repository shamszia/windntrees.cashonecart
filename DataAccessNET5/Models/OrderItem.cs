using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("OrderItem")]
    public partial class OrderItem
    {
        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [Column("OrderID")]
        public Guid? OrderId { get; set; }
        [Column("ItemID")]
        public Guid ItemId { get; set; }
        [Required]
        [StringLength(100)]
        public string ItemName { get; set; }
        [Required]
        [StringLength(100)]
        public string ItemCode { get; set; }
        public short Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal Cost { get; set; }
        [Column(TypeName = "money")]
        public decimal? Discount { get; set; }
        public byte[] RowVersion { get; set; }
        [StringLength(100)]
        public string Description { get; set; }

        [ForeignKey(nameof(ItemId))]
        [InverseProperty(nameof(Product.OrderItems))]
        public virtual Product Item { get; set; }
        [ForeignKey(nameof(OrderId))]
        [InverseProperty("OrderItems")]
        public virtual Order Order { get; set; }
    }
}
