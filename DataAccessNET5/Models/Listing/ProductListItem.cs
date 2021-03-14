using System;

namespace DataAccessNET5.Listing
{
    /// <summary>
    /// Product list item.
    /// </summary>
    public class ProductListItem
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string AccountExpense { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? Expense { get; set; }
    }
}
