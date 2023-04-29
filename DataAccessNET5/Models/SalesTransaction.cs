using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("SalesTransaction")]
    [Index(nameof(TransactionTime), Name = "IX_SalesTransaction_TransactionTime")]
    public partial class SalesTransaction
    {
        public SalesTransaction()
        {
            SalesPayments = new HashSet<SalesPayment>();
            StockSolds = new HashSet<StockSold>();
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
        [Column("BuyerID")]
        public Guid BuyerId { get; set; }
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
        [StringLength(100)]
        public string IntegrationNumber1 { get; set; }
        [StringLength(100)]
        public string IntegrationNumber2 { get; set; }
        [StringLength(100)]
        public string IntegrationMessage { get; set; }
        [StringLength(100)]
        public string IntegrationError { get; set; }
        [StringLength(100)]
        public string TaxNumber { get; set; }
        [Column(TypeName = "money")]
        public decimal? OtherTax { get; set; }
        [StringLength(1)]
        public string PaymentMode { get; set; }
        [StringLength(1)]
        public string InvoiceType { get; set; }
        [Column("ReferenceID")]
        [StringLength(100)]
        public string ReferenceId { get; set; }

        [ForeignKey(nameof(BuyerId))]
        [InverseProperty(nameof(Company.SalesTransactions))]
        public virtual Company Buyer { get; set; }
        [InverseProperty(nameof(SalesPayment.Transaction))]
        public virtual ICollection<SalesPayment> SalesPayments { get; set; }
        [InverseProperty(nameof(StockSold.Stock))]
        public virtual ICollection<StockSold> StockSolds { get; set; }
    }
}
