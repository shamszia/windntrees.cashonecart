using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("StockPurchased")]
    [Index(nameof(StockTime), Name = "IX_StockPurchased_StockTime")]
    public partial class StockPurchased
    {
        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [Column("StockID")]
        public Guid? StockId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StockTime { get; set; }
        [Column("PlaceID")]
        public Guid? PlaceId { get; set; }
        [StringLength(100)]
        public string PlaceName { get; set; }
        [Column("ProductID")]
        public Guid ProductId { get; set; }
        [StringLength(100)]
        public string ProductName { get; set; }
        [Column("PurchaseID")]
        [StringLength(100)]
        public string PurchaseId { get; set; }
        [Column("LegalID")]
        [StringLength(100)]
        public string LegalId { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal SalesPrice { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Commission { get; set; }
        [Column(TypeName = "money")]
        public decimal? IncomeTax { get; set; }
        public byte[] RowVersion { get; set; }

        [ForeignKey(nameof(PlaceId))]
        [InverseProperty("StockPurchaseds")]
        public virtual Place Place { get; set; }
        [ForeignKey(nameof(ProductId))]
        [InverseProperty("StockPurchaseds")]
        public virtual Product Product { get; set; }
        [ForeignKey(nameof(StockId))]
        [InverseProperty(nameof(StockTransaction.StockPurchaseds))]
        public virtual StockTransaction Stock { get; set; }
    }
}
