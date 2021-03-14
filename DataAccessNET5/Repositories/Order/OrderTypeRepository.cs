using DataAccessNET5.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Order
{
    public class OrderTypeRepository : EntityRepository<OrderType>
    {
        public OrderTypeRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public OrderTypeRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {

        }

        protected override OrderType GenerateNewKey(OrderType contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<OrderType> QueryRecords(IQueryable<OrderType> query, SearchInput searchQuery = null)
        {
            Expression<Func<OrderType, bool>> condition = null;

            if (searchQuery != null)
            {
                if (!string.IsNullOrEmpty(searchQuery.key))
                {
                    condition = l => l.UserId == searchQuery.key;
                    query = query.Where(condition);
                }

                searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;

                condition = l => (l.Name.Contains(searchQuery.keyword) || l.Description.Contains(searchQuery.keyword));
                query = query.Where(condition);
            }

            return query;
        }

        protected override IOrderedQueryable<OrderType> SortRecords(IQueryable<OrderType> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<OrderType> orderInterface = null;
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
