using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("StockPayment")]
    [Index(nameof(PaymentTime), Name = "IX_StockPayment_PaymentTime")]
    public partial class StockPayment
    {
        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [Column("TransactionID")]
        public Guid TransactionId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PaymentTime { get; set; }
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }
        [StringLength(255)]
        public string Note { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; }
        public byte[] RowVersion { get; set; }

        [ForeignKey(nameof(TransactionId))]
        [InverseProperty(nameof(StockTransaction.StockPayments))]
        public virtual StockTransaction Transaction { get; set; }
    }
}
