using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("ExpenseType")]
    public partial class ExpenseType
    {
        public ExpenseType()
        {
            Expenses = new HashSet<Expense>();
        }

        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; }
        public byte[] RowVersion { get; set; }

        [InverseProperty(nameof(Expense.ExpenseNavigation))]
        public virtual ICollection<Expense> Expenses { get; set; }
    }
}
