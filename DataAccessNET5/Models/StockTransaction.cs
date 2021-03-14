using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("StockTransaction")]
    [Index(nameof(TransactionTime), Name = "IX_StockTransaction_TransactionTime")]
    public partial class StockTransaction
    {
        public StockTransaction()
        {
            StockPayments = new HashSet<StockPayment>();
            StockPurchaseds = new HashSet<StockPurchased>();
        }

        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime TransactionTime { get; set; }
        [StringLength(100)]
        public string RefName { get; set; }
        public long RefNumber { get; set; }
        public long? LinkTransaction { get; set; }
        [Column("SupplierID")]
        public Guid SupplierId { get; set; }
        [StringLength(200)]
        public string Note { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        public bool Active { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; }
        [Column(TypeName = "money")]
        public decimal? DiscountAmount { get; set; }
        public bool? Credit { get; set; }
        public byte[] RowVersion { get; set; }

        [ForeignKey(nameof(SupplierId))]
        [InverseProperty(nameof(Company.StockTransactions))]
        public virtual Company Supplier { get; set; }
        [InverseProperty(nameof(StockPayment.Transaction))]
        public virtual ICollection<StockPayment> StockPayments { get; set; }
        [InverseProperty(nameof(StockPurchased.Stock))]
        public virtual ICollection<StockPurchased> StockPurchaseds { get; set; }
    }
}
