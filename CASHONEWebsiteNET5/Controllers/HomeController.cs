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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WindnTrees.Abstraction.Core.Controllers;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace Application.Controllers
{
    [Authorize]
    public class HomeController : CRUDLController<Product>
    {
        private ApplicationSettings _applicationSettings;
        private readonly ILogger<HomeController> _logger;

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

        public HomeController(ILogger<HomeController> logger, IOptions<ApplicationSettings> applicationSettings)
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
            return View(new HomeViewModel { Categories = categories, Products = topProducts });
        }

        [AllowAnonymous]
        public IActionResult Product(string id)
        {
            var categories = CategoryRepository.List(new SearchInput { page = 1, size = 10 });
            var product = ((ProductRepository)GetRepository()).Read(id);

            ViewData.Add("CurrencySymbol", _applicationSettings.CurrencySymbol);
            return View(new ProductViewModel { Categories = categories, Product = product });
        }

        [AllowAnonymous]
        public IActionResult Contact()
        {
            ViewData.Add("CurrencySymbol", _applicationSettings.CurrencySymbol);
            return View(new ContactViewModel { Company = _applicationSettings.Company,
            CompanyTitle = _applicationSettings.CompanyTitle,
            Website = _applicationSettings.Website,
            BriefDescription = _applicationSettings.BreifDescription,
            AddressLine1 = _applicationSettings.AddressLine1,
            AddressLine2 = _applicationSettings.AddressLine2,
            AddressLine3 = _applicationSettings.AddressLine3,
            Email = _applicationSettings.ContactEmail,
            Phone = _applicationSettings.ContactPhone,
            Cell = _applicationSettings.ContactCell,
            BusinessHours1 = _applicationSettings.BusinessHourLine1,
            BusinessHours2 = _applicationSettings.BusinessHourLine2
            });
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetPicture(string id)
        {
            var content = ((ProductRepository)GetRepository()).Read(id);
            return File(content.Picture, "application/binary", string.Format("{0}.png", id));
        }
    }
}
