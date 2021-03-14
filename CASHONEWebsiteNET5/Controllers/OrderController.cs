using Application.Models.Configuration;
using DataAccessNET5.Models;
using DataAccessNET5.Repositories.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindnTrees.Abstraction.Core.Controllers;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace Application.Controllers
{
    [Authorize]
    public class OrderController : CRUDLController<Order>
    {
        private ApplicationSettings _applicationSettings;
        private readonly ILogger<HomeController> _logger;

        protected override CRUDLMRepository<Order> GetRepository()
        {
            return new OrderRepository(Startup.Configuration.GetConnectionString("DatabaseConnection"));
        }

        public OrderController(ILogger<HomeController> logger, IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings.Value;
            _logger = logger;

        }

        [AllowAnonymous]
        public override JsonResult Create([FromBody] Order contentObject)
        {
            return base.Create(contentObject);
        }

        protected override Order PostCreate(Order contentObject)
        {
            HttpContext.Session.Remove("CASHONECart");
            return base.PostCreate(contentObject);
        }
        
        [AllowAnonymous]
        public IActionResult Verify(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewData.Add("OrderResultMessage", "");
            }
            else
            {
                try
                {
                    long orderNo = long.Parse(id);
                    ViewData.Add("OrderResultMessage", "Thank you, your order have been received.");
                }
                catch
                {
                    return View("Error");
                }

                ViewData.Add("OrderNo", id);
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Result([FromForm]SearchInput searchInput)
        {
            if (!string.IsNullOrEmpty(searchInput.keyword))
            {
                var order = ((OrderRepository)GetRepository()).Read(searchInput.key);

                if (order == null)
                {
                    return View("Verify");
                }
                else
                {
                    if (searchInput.keyword.Equals(order.SecretWord, StringComparison.Ordinal))
                    {
                        decimal grandTotal = 0;
                        decimal discountTotal = 0;
                        foreach (var orderItem in order.OrderItems)
                        {
                            grandTotal += orderItem.LineTotal;
                            discountTotal += (orderItem.Quantity * orderItem.ItemDiscount);
                        }

                        ViewData.Add("CurrencySymbol", _applicationSettings.CurrencySymbol);
                        return View(new Application.Models.Cart.OrderResultViewModel { Order = order, GrandTotal = grandTotal, DiscountTotal = discountTotal });
                    }
                }

                ViewData.Add("OrderResultMessage", "Error, invalid order details.");
                return View("Verify");
            }

            ViewData.Add("OrderResultMessage", "Error, invalid order details.");
            return View("Verify");
        }
    }
}
