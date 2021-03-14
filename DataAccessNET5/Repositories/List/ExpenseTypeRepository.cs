using System;
using System.Linq;
using System.Linq.Expressions;
using DataAccessNET5.Models;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.List
{
    /// <summary>
    /// ExpenseType repository.
    /// </summary>
    public class ExpenseTypeRepository : EntityRepository<ExpenseType>
    {
        public ExpenseTypeRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public ExpenseTypeRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override ExpenseType GenerateNewKey(ExpenseType contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<ExpenseType> QueryRecords(IQueryable<ExpenseType> query, SearchInput searchQuery = null)
        {
            Expression<Func<ExpenseType, bool>> condition = null;

            searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;

            condition = l => (l.Name.Contains(searchQuery.keyword) || l.Description.Contains(searchQuery.keyword));
            query = query.Where(condition);
            return query;
        }

        protected override IOrderedQueryable<ExpenseType> SortRecords(IQueryable<ExpenseType> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<ExpenseType> orderInterface = null;
            if (searchQuery != null)
            {
                if (searchQuery.descend == null ? false : ((bool)searchQuery.descend))
                {
                    orderInterface = query.OrderByDescending(l => l.Name);
                }
                else
                {
                    orderInterface = query.OrderBy(l => l.Name);
                }
            }
            return orderInterface;
        }
    }
}
