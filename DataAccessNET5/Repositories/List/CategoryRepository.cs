using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DataAccessNET5.Models;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.List
{
    /// <summary>
    /// Category repository.
    /// </summary>
    public class CategoryRepository : EntityRepository<Category>
    {
        public CategoryRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public CategoryRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override Category GenerateNewKey(Category contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<Category> QueryRecords(IQueryable<Category> query, SearchInput searchQuery = null)
        {
            Expression<Func<Category, bool>> condition = null;
            if (searchQuery != null)
            {
                if (!string.IsNullOrEmpty(searchQuery.key))
                {
                    condition = l => l.Tag == searchQuery.key;
                    query = query.Where(condition);
                }

                searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;

                condition = l => (l.Name.Contains(searchQuery.keyword) || l.Description.Contains(searchQuery.keyword));
                query = query.Where(condition);
            }

            return query;
        }

        protected override IOrderedQueryable<Category> SortRecords(IQueryable<Category> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<Category> orderInterface = null;
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
