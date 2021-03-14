using DataAccessNET5.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Order
{
    public class OrderItemRepository : EntityRepository<OrderItem>
    {
        public OrderItemRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public OrderItemRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {

        }

        protected override OrderItem GenerateNewKey(OrderItem contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<OrderItem> QueryRecords(IQueryable<OrderItem> query, SearchInput searchQuery = null)
        {
            Expression<Func<OrderItem, bool>> condition = null;

            Guid? orderID = null;
            try
            {
                orderID = Guid.Parse(searchQuery.key);
                condition = l => (l.OrderId == orderID);
                query = query.Where(condition);
            }
            catch { }

            long orderNumber = 0;
            try
            {
                if (relatedObjects.Contains("Order"))
                {
                    orderNumber = long.Parse(searchQuery.keyword);
                    condition = l => l.Order.OrderNo == orderNumber;
                    query = query.Where(condition);
                }
            }
            catch { }

            if (orderNumber == 0)
            {
                searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;
                condition = l => (l.ItemName.Contains(searchQuery.keyword) || l.ItemCode.Contains(searchQuery.keyword));
                query = query.Where(condition);
            }

            return query;
        }

        protected override IOrderedQueryable<OrderItem> SortRecords(IQueryable<OrderItem> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<OrderItem> orderInterface = null;
            if (searchQuery != null)
            {
                if (searchQuery.descend == null ? false : ((bool)searchQuery.descend))
                {
                    orderInterface = query.OrderByDescending(l => l.Quantity);
                }
                else
                {
                    orderInterface = query.OrderBy(l => l.Quantity);
                }
            }
            return orderInterface;
        }
    }
}
