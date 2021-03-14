using DataAccessNET5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models.Cart
{
    public class OrderViewModel
    {
        public List<Category> Categories = new List<Category>();
        public List<Product> Products = new List<Product>();
    }
}
