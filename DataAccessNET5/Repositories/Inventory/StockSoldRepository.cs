using System;
using System.Linq;
using System.Linq.Expressions;
using DataAccessNET5.Models;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Inventory
{
    /// <summary>
    /// Gets stock sold repository.
    /// </summary>
    public class StockSoldRepository : EntityRepository<StockSold>
    {
        public StockSoldRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public StockSoldRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override StockSold GenerateNewKey(StockSold contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<StockSold> QueryRecords(IQueryable<StockSold> query, SearchInput searchQuery = null)
        {
            Expression<Func<StockSold, bool>> condition = null;

            if (searchQuery.starttime >= new DateTime(1800, 1, 1) && searchQuery.endtime >= new DateTime(1800, 1, 1))
            {
                condition = l => (l.StockTime >= searchQuery.starttime && l.StockTime < searchQuery.endtime);
                query = query.Where(condition);
            }

            if (!string.IsNullOrEmpty(searchQuery.key))
            {
                Guid? transactionId = null;
                try
                {
                    transactionId = Guid.Parse(searchQuery.key);
                }
                catch { }

                condition = l => (l.StockId == transactionId);
                query = query.Where(condition);
            }

            searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;
            condition = l => (l.Product.Name.Contains(searchQuery.keyword) || l.Description.Contains(searchQuery.keyword));
            query = query.Where(condition);

            return query;
        }

        protected override IOrderedQueryable<StockSold> SortRecords(IQueryable<StockSold> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<StockSold> orderInterface = null;
            if (searchQuery != null)
            {
                if (searchQuery.descend == null ? false : ((bool)searchQuery.descend))
                {
                    orderInterface = query.OrderByDescending(l => l.StockTime);
                }
                else
                {
                    orderInterface = query.OrderBy(l => l.StockTime);
                }
            }
            return orderInterface;
        }
    }
}
