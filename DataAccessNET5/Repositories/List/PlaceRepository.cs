using System;
using System.Linq;
using System.Linq.Expressions;
using DataAccessNET5.Models;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.List
{
    /// <summary>
    /// Place repository.
    /// </summary>
    public class PlaceRepository : EntityRepository<Place>
    {
        public PlaceRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public PlaceRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override Place GenerateNewKey(Place contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<Place> QueryRecords(IQueryable<Place> query, SearchInput searchQuery = null)
        {
            Expression<Func<Place, bool>> condition = null;
            if (searchQuery != null)
            {
                searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;

                condition = l => (l.Name.Contains(searchQuery.keyword) || l.Description.Contains(searchQuery.keyword));
                query = query.Where(condition);
            }
            return query;
        }

        protected override IOrderedQueryable<Place> SortRecords(IQueryable<Place> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<Place> orderInterface = null;
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
