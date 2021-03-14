using DataAccessNET5.Models;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Inventory
{
    /// <summary>
    /// Company repository.
    /// </summary>
    public class CompanyRepository : EntityRepository<Company>
    {
        public CompanyRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public CompanyRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override Company GenerateNewKey(Company contentObject)
        {
            contentObject.Uid = Guid.NewGuid();

            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        public override Company Read(object id)
        {
            try
            {
                Guid.Parse(id.ToString());
                return base.Read(id);
            }
            catch
            { }

            IQueryable<Company> iQuery = entitySet;
            var company = iQuery.Where(l => (l.Cell == id.ToString() || l.LegalName == id.ToString())).SingleOrDefault();

            return company;
        }

        protected override IQueryable<Company> QueryRecords(IQueryable<Company> query, SearchInput searchQuery = null)
        {
            Expression<Func<Company, bool>> condition = null;

            if (searchQuery != null)
            {
                if (!string.IsNullOrEmpty(searchQuery.key))
                {
                    condition = l => (l.UserId == searchQuery.key);
                    query = query.Where(condition);
                }

                searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;

                condition = l => (l.LegalCode.Contains(searchQuery.keyword) || l.LegalName.Contains(searchQuery.keyword) || l.Ntn.Contains(searchQuery.keyword) || l.Strn.Contains(searchQuery.keyword) || l.Phone.Contains(searchQuery.keyword) || l.Cell.Contains(searchQuery.keyword) || l.Email.Contains(searchQuery.keyword) || l.Address.Contains(searchQuery.keyword));
                query = query.Where(condition);
            }

            return query;
        }

        protected override IOrderedQueryable<Company> SortRecords(IQueryable<Company> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<Company> orderInterface = null;
            if (searchQuery != null)
            {
                if (searchQuery.descend == null ? false : ((bool)searchQuery.descend))
                {
                    orderInterface = query.OrderByDescending(l => l.LegalName);
                }
                else
                {
                    orderInterface = query.OrderBy(l => l.LegalName);
                }
            }
            return orderInterface;
        }
    }
}
