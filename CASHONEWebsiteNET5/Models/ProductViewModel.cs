﻿using DataAccessNET5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ProductViewModel
    {
        public List<Category> Categories = new List<Category>();
        public Product Product { get; set; }
    }
}
