using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("Company")]
    [Index(nameof(Cell), Name = "IX_Company_CellNo", IsUnique = true)]
    public partial class Company
    {
        public Company()
        {
            SalesTransactions = new HashSet<SalesTransaction>();
            StockTransactions = new HashSet<StockTransaction>();
        }

        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; }
        [StringLength(200)]
        public string Address { get; set; }
        [Required]
        [StringLength(20)]
        public string Cell { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        [StringLength(100)]
        public string Country { get; set; }
        [StringLength(1024)]
        public string Description { get; set; }
        [StringLength(128)]
        public string Email { get; set; }
        [StringLength(100)]
        public string LegalCode { get; set; }
        [Required]
        [StringLength(200)]
        public string LegalName { get; set; }
        [Column("NTN")]
        [StringLength(100)]
        public string Ntn { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        public byte[] RowVersion { get; set; }
        [Column("STRN")]
        [StringLength(100)]
        public string Strn { get; set; }
        [StringLength(100)]
        public string Secretary { get; set; }
        [StringLength(100)]
        public string State { get; set; }
        [StringLength(10)]
        public string PostalCode { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RegistrationTime { get; set; }

        [InverseProperty(nameof(SalesTransaction.Buyer))]
        public virtual ICollection<SalesTransaction> SalesTransactions { get; set; }
        [InverseProperty(nameof(StockTransaction.Supplier))]
        public virtual ICollection<StockTransaction> StockTransactions { get; set; }
    }
}
