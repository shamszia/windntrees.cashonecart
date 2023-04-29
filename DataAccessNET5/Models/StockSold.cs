using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("StockSold")]
    [Index(nameof(StockTime), Name = "IX_StockSold_StockTime")]
    public partial class StockSold
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
        [Column("SalesID")]
        [StringLength(100)]
        public string SalesId { get; set; }
        [Column("LegalID")]
        [StringLength(100)]
        public string LegalId { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal? Expense { get; set; }
        [Column(TypeName = "money")]
        public decimal? SalesPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal? SalesTax { get; set; }
        public byte[] RowVersion { get; set; }
        public int? OpenQuantity { get; set; }
        public int? PackQuantity { get; set; }
        public double? Discount { get; set; }
        public double? OtherTax { get; set; }
        [Column("ReferenceID")]
        [StringLength(100)]
        public string ReferenceId { get; set; }
        [StringLength(100)]
        public string InventoryCode { get; set; }
        [StringLength(100)]
        public string LegalCode { get; set; }
        [Column(TypeName = "money")]
        public decimal? IncomeTax { get; set; }
        public double? TaxRate { get; set; }

        [ForeignKey(nameof(PlaceId))]
        [InverseProperty("StockSolds")]
        public virtual Place Place { get; set; }
        [ForeignKey(nameof(ProductId))]
        [InverseProperty("StockSolds")]
        public virtual Product Product { get; set; }
        [ForeignKey(nameof(StockId))]
        [InverseProperty(nameof(SalesTransaction.StockSolds))]
        public virtual SalesTransaction Stock { get; set; }
    }
}
