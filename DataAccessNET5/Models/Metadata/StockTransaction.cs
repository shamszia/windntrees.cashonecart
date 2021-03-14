using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessNET5.Models
{
    public partial class StockTransaction
    {
        [NotMapped]
        public string LegalName { get; set; }

        [NotMapped]
        public decimal TotalPurchase { get; set; }

        [NotMapped]
        public decimal DiscountTotal { get; set; }

        [NotMapped]
        public decimal TotalPayments { get; set; }
    }
}
