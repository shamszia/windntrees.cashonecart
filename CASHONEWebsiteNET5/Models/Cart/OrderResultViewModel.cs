using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models.Cart
{
    public class OrderResultViewModel
    {
        public DataAccessNET5.Models.Order Order { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal GrandTotal { get; set; }
    }
}
