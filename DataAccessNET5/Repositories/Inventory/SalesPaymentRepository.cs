using System;
using System.Linq;
using System.Linq.Expressions;
using DataAccessNET5.Models;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Inventory
{
    /// <summary>
    /// SalesPayment repository.
    /// </summary>
    public class SalesPaymentRepository : EntityRepository<SalesPayment>
    {
        public SalesPaymentRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public SalesPaymentRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override SalesPayment GenerateNewKey(SalesPayment contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<SalesPayment> QueryRecords(IQueryable<SalesPayment> query, SearchInput searchQuery = null)
        {
            Expression<Func<SalesPayment, bool>> condition = null;
            if (!string.IsNullOrEmpty(searchQuery.key))
            {
                Guid? transactionId = null;
                try
                {
                    transactionId = Guid.Parse(searchQuery.key);
                }
                catch { }

                if (transactionId != null)
                {
                    condition = l => (l.TransactionId == transactionId);
                    query = query.Where(condition);
                }
            }

            searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;
            query = query.Where(l => l.Note == searchQuery.keyword);

            return query;
        }

        protected override IOrderedQueryable<SalesPayment> SortRecords(IQueryable<SalesPayment> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<SalesPayment> orderInterface = null;
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