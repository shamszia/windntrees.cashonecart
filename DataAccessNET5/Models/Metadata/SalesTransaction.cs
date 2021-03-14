using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessNET5.Models
{
    public partial class SalesTransaction
    {
        [NotMapped]
        public string LegalName { get; set; }

        [NotMapped]
        public decimal TotalSales { get; set; }

        [NotMapped]
        public decimal DiscountTotal { get; set; }

        [NotMapped]
        public decimal TotalPayments { get; set; }
    }
}
