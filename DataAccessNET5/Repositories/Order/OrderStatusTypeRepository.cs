using DataAccessNET5.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Order
{
    public class OrderStatusTypeRepository : EntityRepository<OrderStatusType>
    {
        public OrderStatusTypeRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public OrderStatusTypeRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {

        }

        protected override OrderStatusType GenerateNewKey(OrderStatusType contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<OrderStatusType> QueryRecords(IQueryable<OrderStatusType> query, SearchInput searchQuery = null)
        {
            Expression<Func<OrderStatusType, bool>> condition = null;

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

        protected override IOrderedQueryable<OrderStatusType> SortRecords(IQueryable<OrderStatusType> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<OrderStatusType> orderInterface = null;
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
