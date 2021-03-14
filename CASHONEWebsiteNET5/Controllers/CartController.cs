using Application.Models.Cart;
using Application.Models.CashoneCart;
using Application.Models.CashoneCart.Repository;
using Application.Models.Configuration;
using DataAccessNET5.Models;
using DataAccessNET5.Repositories.Inventory;
using DataAccessNET5.Repositories.List;
using DataAccessNET5.Repositories.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using WindnTrees.Abstraction.Core.Controllers;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace Application.Controllers
{   
    public class CartController : CRUDLController<CartItem>
    {
        private ApplicationSettings _applicationSettings;
        private readonly ILogger<HomeController> _logger;

        #region GetSessionCartRepository
        private SessionCartRepository m_SessionCartRepository;
        private SessionCartRepository GetSessionCartRepository
        {
            get
            {
                if (m_SessionCartRepository == null)
                {
                    m_SessionCartRepository = new SessionCartRepository();
                }
                return m_SessionCartRepository;
            }
        } 
        #endregion

        protected override CRUDLMRepository<CartItem> GetRepository()
        {
            return GetSessionCartRepository;
        }

        private ProductRepository ProductRepository
        {
            get
            {
                return new ProductRepository(Startup.Configuration.GetConnectionString("DatabaseConnection"));
            }
        }

        private CategoryRepository CategoryRepository
        {
            get
            {
                return new CategoryRepository(Startup.Configuration.GetConnectionString("DatabaseConnection"));
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            ((SessionCartRepository)GetRepository()).Session = context.HttpContext.Session;
            ((SessionCartRepository)GetRepository()).Principal = context.HttpContext.User;
        }

        public CartController(ILogger<HomeController> logger, IOptions<ApplicationSettings> applicationSettings)
        {
            _logger = logger;
            _applicationSettings = applicationSettings.Value;
            m_TargetRedirection = true;
        }

        #region Cart_Pages
        public IActionResult Index()
        {
            var categories = CategoryRepository.List(new SearchInput { page = 1, size = 10 });
            var topProducts = ProductRepository.GetProductsWithQuantity(new WindnTrees.ICRUDS.Standard.SearchInput
            {
                page = 1,
                size = 10,
                options = new List<SearchField> {
                new SearchField { field = "top", value = "" }
                }
            });

            ViewData.Add("CurrencySymbol", _applicationSettings.CurrencySymbol);
            return View(new CartViewModel { Categories = categories, Products = topProducts });
        }

        public IActionResult Order()
        {
            var categories = CategoryRepository.List(new SearchInput { page = 1, size = 10 });

            ViewData.Add("CurrencySymbol", _applicationSettings.CurrencySymbol);

            return View(new OrderViewModel { Categories = categories, Products = new List<Product>() });
        }
        #endregion

        #region Cart_Operations
        public JsonResult GetCart(string id)
        {
            //id parameter is for API compliance
            return GetObjectResult(((SessionCartRepository)GetRepository()).GetCart(id), null);
        }

        public JsonResult UpdateCart(Cart cart)
        {
            return GetObjectResult(((SessionCartRepository)GetRepository()).UpdateCart(cart), null);
        }

        public JsonResult EmptyCart(Cart cart)
        {
            return GetObjectResult(((SessionCartRepository)GetRepository()).EmptyCart(cart), null);
        }

        public JsonResult GetCartCount(string id)
        {
            //id parameter is for API compliance
            return GetObjectResult(((SessionCartRepository)GetRepository()).GetCartCount(), null);
        }
        #endregion
    }
}