using System;
using System.Linq;
using System.Linq.Expressions;
using DataAccessNET5.Models;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Inventory
{
    /// <summary>
    /// StockPayment repository.
    /// </summary>
    public class StockPaymentRepository : EntityRepository<StockPayment>
    {
        public StockPaymentRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public StockPaymentRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override StockPayment GenerateNewKey(StockPayment contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<StockPayment> QueryRecords(IQueryable<StockPayment> query, SearchInput searchQuery = null)
        {
            Expression<Func<StockPayment, bool>> condition = null;
            if (!string.IsNullOrEmpty(searchQuery.key))
            {
                Guid? transactionId = null;
                try
                {
                    transactionId = Guid.Parse(searchQuery.key);
                }
                catch { }

                condition = l => (l.TransactionId == transactionId);
                query = query.Where(condition);
            }
            return query;
        }

        protected override IOrderedQueryable<StockPayment> SortRecords(IQueryable<StockPayment> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<StockPayment> orderInterface = null;
            if (searchQuery != null)
            {
                if (searchQuery.descend == null ? false : ((bool)searchQuery.descend))
                {
                    orderInterface = query.OrderByDescending(l => l.PaymentTime);
                }
                else
                {
                    orderInterface = query.OrderBy(l => l.PaymentTime);
                }
            }
            return orderInterface;
        }
    }
}
