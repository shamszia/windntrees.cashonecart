using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DataAccessNET5.Models
{
    [Table("Product")]
    [Index(nameof(Code), Name = "IX_Product_Code", IsUnique = true)]
    [Index(nameof(Name), Name = "IX_Product_Name", IsUnique = true)]
    public partial class Product
    {
        public Product()
        {
            OrderItems = new HashSet<OrderItem>();
            StockPurchaseds = new HashSet<StockPurchased>();
            StockSolds = new HashSet<StockSold>();
        }

        [Key]
        [Column("UID")]
        public Guid Uid { get; set; }
        [StringLength(100)]
        public string Reference { get; set; }
        [Column("UserID")]
        [StringLength(450)]
        public string UserId { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [StringLength(100)]
        public string Manufacturer { get; set; }
        [StringLength(100)]
        public string Code { get; set; }
        [StringLength(100)]
        public string Color { get; set; }
        [StringLength(1024)]
        public string Description { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string SecondTitle { get; set; }
        [Column(TypeName = "image")]
        public byte[] Picture { get; set; }
        public int? StockLevel { get; set; }
        [Column(TypeName = "money")]
        public decimal? PublishedPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal? SalesPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal? PurchaseCost { get; set; }
        [Column(TypeName = "money")]
        public decimal? Discount { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Commission { get; set; }
        [Column(TypeName = "money")]
        public decimal? IncomeTax { get; set; }
        [Column(TypeName = "money")]
        public decimal? SalesTax { get; set; }
        public byte[] RowVersion { get; set; }
        public bool? Published { get; set; }
        public bool? Top { get; set; }
        public bool? Favourite { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RegistrationTime { get; set; }
        public double? FurtherTax { get; set; }
        public double? SalesTaxRate { get; set; }
        public double? IncomeTaxRate { get; set; }

        [InverseProperty(nameof(OrderItem.Item))]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        [InverseProperty(nameof(StockPurchased.Product))]
        public virtual ICollection<StockPurchased> StockPurchaseds { get; set; }
        [InverseProperty(nameof(StockSold.Product))]
        public virtual ICollection<StockSold> StockSolds { get; set; }
    }
}
