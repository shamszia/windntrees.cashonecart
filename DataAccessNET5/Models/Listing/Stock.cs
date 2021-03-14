using System;

namespace DataAccessNET5.Models.Listing
{
    /// <summary>
    /// Stock model.
    /// </summary>
    public partial class Stock
    {
        public string StockTime { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid PlaceId { get; set; }
        public string PlaceName { get; set; }
        public string Account { get; set; }
        public int Quantity { get; set; }
        public decimal SalesPrice { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? IncomeTax { get; set; }
    }
}
