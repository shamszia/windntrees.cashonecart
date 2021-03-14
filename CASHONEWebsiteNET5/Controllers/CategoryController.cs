using DataAccessNET5.Models;
using DataAccessNET5.Repositories.List;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class CategoryController : CRUDLController<Category>
    {
        protected override CRUDLMRepository<Category> GetRepository()
        {
            return new CategoryRepository(Startup.Configuration.GetConnectionString("DatabaseConnection"));
        }

        public CategoryController()
        {
            m_TargetRedirection = true;
        }

        [AllowAnonymous]
        public override JsonResult List([FromBody] WebInput webInput)
        {
            return base.List(webInput);
        }
    }
}
