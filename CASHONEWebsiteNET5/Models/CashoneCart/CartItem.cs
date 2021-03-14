using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models.CashoneCart
{
    [Serializable]
    public class CartItem : IComparable
    {
        [Key]
        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public short AvailableQuantity { get; set; }
        public short Quantity { get; set; }
        public decimal Cost { get; set; }
        public decimal? Discount { get; set; }

        public int CompareTo(object obj)
        {
            return ItemName.CompareTo(((CartItem)obj).ItemName);
        }
        public override int GetHashCode()
        {
            return ItemId.GetHashCode();
        }
    }
}
