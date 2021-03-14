using Application.Models;
using Application.Models.Configuration;
using DataAccessNET5.Models;
using DataAccessNET5.Repositories.Inventory;
using DataAccessNET5.Repositories.List;
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
    public class ProductController : CRUDLController<DataAccessNET5.Models.Product>
    {
        private ApplicationSettings _applicationSettings;
        private readonly ILogger<ProductController> _logger;

        protected override CRUDLMRepository<Product> GetRepository()
        {
            return new ProductRepository(Startup.Configuration.GetConnectionString("DatabaseConnection"));
        }

        private CategoryRepository CategoryRepository
        {
            get
            {
                return new CategoryRepository(Startup.Configuration.GetConnectionString("DatabaseConnection"));
            }
        }

        public ProductController(ILogger<ProductController> logger, IOptions<ApplicationSettings> applicationSettings)
        {
            _applicationSettings = applicationSettings.Value;
            _logger = logger;

            m_TargetRedirection = true;
        }

        [AllowAnonymous]
        public override JsonResult List([FromBody] WebInput webInput)
        {
            return base.List(webInput);
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            var categories = CategoryRepository.List(new SearchInput { page = 1, size = 10 });
            var topProducts = ((ProductRepository)GetRepository()).GetProductsWithQuantity(new WindnTrees.ICRUDS.Standard.SearchInput
            {
                page = 1,
                size = 10,
                options = new List<SearchField> {
                new SearchField { field = "top", value = "" }
                }
            });

            ViewData.Add("CurrencySymbol", _applicationSettings.CurrencySymbol);
            return View(new TopViewModel { Categories = categories, Products = topProducts });
        }

        [AllowAnonymous]
        public IActionResult Favourite()
        {
            var categories = CategoryRepository.List(new SearchInput { page = 1, size = 10 });
            var favouriteProducts = ((ProductRepository)GetRepository()).GetProductsWithQuantity(new WindnTrees.ICRUDS.Standard.SearchInput
            {
                page = 1,
                size = 10,
                options = new List<SearchField> {
                new SearchField { field = "favourite", value = "" }
                }
            });

            ViewData.Add("CurrencySymbol", _applicationSettings.CurrencySymbol);
            return View(new FavouritesViewModel { Categories = categories, Products = favouriteProducts });
        }

        [AllowAnonymous]
        public IActionResult Top()
        {
            var categories = CategoryRepository.List(new SearchInput { page = 1, size = 10 });
            var topProducts = ((ProductRepository)GetRepository()).GetProductsWithQuantity(new WindnTrees.ICRUDS.Standard.SearchInput
            {
                page = 1,
                size = 10,
                options = new List<SearchField> {
                new SearchField { field = "top", value = "" }
                }
            });

            ViewData.Add("CurrencySymbol", _applicationSettings.CurrencySymbol);
            return View(new TopViewModel { Categories = categories, Products = topProducts });
        }

        [AllowAnonymous]
        public IActionResult Discount()
        {
            var categories = CategoryRepository.List(new SearchInput { page = 1, size = 10 });
            var discountProducts = ((ProductRepository)GetRepository()).GetProductsWithQuantity(new WindnTrees.ICRUDS.Standard.SearchInput
            {
                page = 1,
                size = 10,
                options = new List<SearchField> {
                new SearchField { field = "discount", value = "" }
                }
            });

            ViewData.Add("CurrencySymbol", _applicationSettings.CurrencySymbol);
            return View(new DiscountViewModel { Categories = categories, Products = discountProducts });
        }

        [AllowAnonymous]
        public IActionResult Category(string id)
        {
            var categories = CategoryRepository.List(new SearchInput { page = 1, size = 10 });
            var categoryProducts = ((ProductRepository)GetRepository()).GetProductsWithQuantity(new WindnTrees.ICRUDS.Standard.SearchInput
            {
                key = id,
                page = 1,
                size = 10
            });

            ViewData.Add("CurrencySymbol", _applicationSettings.CurrencySymbol);
            return View(new CategoryProductsViewModel { Categories = categories, Products = categoryProducts });
        }
    }
}
