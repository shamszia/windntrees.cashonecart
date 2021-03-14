using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("Expense")]
    [Index(nameof(ExpenseTime), Name = "IX_Expense_ExpenseTime")]
    public partial class Expense
    {
        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [Column("ExpenseID")]
        public Guid ExpenseId { get; set; }
        [Column("PlaceID")]
        public Guid? PlaceId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpenseTime { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }
        [Column(TypeName = "money")]
        public decimal? RemainingAmount { get; set; }
        [Column(TypeName = "money")]
        public decimal? SalesTax { get; set; }
        [Column(TypeName = "money")]
        public decimal? IncomeTax { get; set; }
        public byte[] RowVersion { get; set; }

        [ForeignKey(nameof(ExpenseId))]
        [InverseProperty(nameof(ExpenseType.Expenses))]
        public virtual ExpenseType ExpenseNavigation { get; set; }
        [ForeignKey(nameof(PlaceId))]
        [InverseProperty("Expenses")]
        public virtual Place Place { get; set; }
    }
}
