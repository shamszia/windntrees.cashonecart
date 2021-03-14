using DataAccessNET5.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Inventory
{
    public class CODSaleRepository : EntityRepository<Codsale>
    {
        public CODSaleRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public CODSaleRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {

        }

        protected override Codsale GenerateNewKey(Codsale contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            contentObject.Codtime = DateTime.UtcNow;
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<Codsale> QueryRecords(IQueryable<Codsale> query, SearchInput searchQuery = null)
        {
            Expression<Func<Codsale, bool>> condition = null;
            if (searchQuery != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(searchQuery.key))
                    {
                        Guid guid = Guid.Parse(searchQuery.key);
                        condition = l => l.Uid == guid;
                        query = query.Where(condition);
                    }
                }
                catch
                {
                    condition = l => l.Reference == searchQuery.key;
                    query = query.Where(condition);
                }

                searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;

                condition = l => (l.Consignee.Contains(searchQuery.keyword) || l.Reference.Contains(searchQuery.keyword) || l.ProductDetail.Contains(searchQuery.keyword) || l.Mobile.Contains(searchQuery.keyword) || l.City.Contains(searchQuery.keyword) || l.Address.Contains(searchQuery.keyword) || l.Remarks.Contains(searchQuery.keyword));
                query = query.Where(condition);
            }

            return query;
        }

        protected override IOrderedQueryable<Codsale> SortRecords(IQueryable<Codsale> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<Codsale> orderInterface = null;
            if (searchQuery != null)
            {
                if (searchQuery.descend == null ? false : ((bool)searchQuery.descend))
                {
                    orderInterface = query.OrderByDescending(l => l.Codtime);
                }
                else
                {
                    orderInterface = query.OrderBy(l => l.Codtime);
                }
            }
            return orderInterface;
        }

        //public override Codsale Update(Codsale contentObject)
        //{
        //    return Update(contentObject, contentObject.Uid.ToString());
        //}

        //public override Codsale Delete(Codsale contentObject)
        //{
        //    return Delete(contentObject, contentObject.Uid.ToString());
        //}
    }
}
