using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("Place")]
    [Index(nameof(Name), Name = "IX_Place_Name", IsUnique = true)]
    public partial class Place
    {
        public Place()
        {
            Expenses = new HashSet<Expense>();
            StockPurchaseds = new HashSet<StockPurchased>();
            StockSolds = new HashSet<StockSold>();
        }

        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RegistrationTime { get; set; }
        public byte[] RowVersion { get; set; }

        [InverseProperty(nameof(Expense.Place))]
        public virtual ICollection<Expense> Expenses { get; set; }
        [InverseProperty(nameof(StockPurchased.Place))]
        public virtual ICollection<StockPurchased> StockPurchaseds { get; set; }
        [InverseProperty(nameof(StockSold.Place))]
        public virtual ICollection<StockSold> StockSolds { get; set; }
    }
}
