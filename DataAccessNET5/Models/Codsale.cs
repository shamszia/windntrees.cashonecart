using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("CODSale")]
    public partial class Codsale
    {
        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [Column("CODTime", TypeName = "datetime")]
        public DateTime Codtime { get; set; }
        [Column("CODCompanyID")]
        public Guid? CodcompanyId { get; set; }
        [Required]
        [StringLength(50)]
        public string Reference { get; set; }
        [Required]
        [StringLength(50)]
        public string Consignee { get; set; }
        [Required]
        [StringLength(16)]
        public string Mobile { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        [StringLength(200)]
        public string Address { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Pieces { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Weight { get; set; }
        [Column(TypeName = "money")]
        public decimal? Amount { get; set; }
        public bool? SpecialHandling { get; set; }
        [StringLength(100)]
        public string ServiceType { get; set; }
        [StringLength(200)]
        public string ProductDetail { get; set; }
        [StringLength(200)]
        public string Remarks { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
