using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using WindnTrees.CRUDS.Repository.Core;
using DataAccessNET5.Models;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Inventory
{
    /// <summary>
    /// Expense repository.
    /// </summary>
    public class ExpenseRepository : EntityRepository<Expense>
    {
        public ExpenseRepository(string connectionString)
            : base(new ApplicationContext(connectionString), "ExpenseNavigation,Place")
        {

        }

        public ExpenseRepository(ApplicationContext dbContext, string relatedObjects = "ExpenseNavigation,Place")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override Expense GenerateNewKey(Expense contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            contentObject.ExpenseTime = DateTime.UtcNow;
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        public override Expense Create(Expense contentObject)
        {
            contentObject.ExpenseNavigation = null;
            contentObject.Place = null;

            return base.Create(contentObject);
        }

        public override Expense Update(Expense contentObject)
        {
            contentObject.ExpenseNavigation = null;
            contentObject.Place = null;

            return Update(contentObject);
        }

        protected override IQueryable<Expense> QueryRecords(IQueryable<Expense> query, SearchInput searchQuery = null)
        {
            Expression<Func<Expense, bool>> condition = null;

            DateTime startTime = searchQuery.starttime.Value.ToUniversalTime();
            DateTime endTime = searchQuery.endtime.Value.ToUniversalTime();

            condition = l => l.ExpenseTime >= startTime && l.ExpenseTime <= endTime;
            query = query.Where(condition);

            if (searchQuery != null)
            {
                if (!string.IsNullOrEmpty(searchQuery.key))
                {
                    Guid? expenseId = null;
                    try
                    {
                        expenseId = Guid.Parse(searchQuery.key);
                        condition = l => (l.Uid == expenseId);
                        query = query.Where(condition);
                    }
                    catch { }
                }

                if (((List<SearchField>)searchQuery.keys) != null)
                {
                    var place = ((List<SearchField>)searchQuery.keys).Where(l => l.field == "Place").SingleOrDefault();
                    if (place != null)
                    {
                        Guid? placeId = null;
                        try
                        {
                            placeId = Guid.Parse(place.value);
                        }
                        catch { }

                        condition = l => (l.PlaceId == placeId);
                        query = query.Where(condition);
                    }

                    var expense = ((List<SearchField>)searchQuery.keys).Where(l => l.field == "Expense").SingleOrDefault();
                    if (expense != null)
                    {
                        Guid? expenseId = null;
                        try
                        {
                            expenseId = Guid.Parse(expense.value);
                        }
                        catch { }

                        condition = l => (l.ExpenseId == expenseId);
                        query = query.Where(condition);
                    }
                }

                searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;

                condition = l => (l.Description.Contains(searchQuery.keyword));
                query = query.Where(condition);
            }
            return query;
        }

        protected override IOrderedQueryable<Expense> SortRecords(IQueryable<Expense> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<Expense> orderInterface = null;
            if (searchQuery != null)
            {
                if (searchQuery.descend == null ? false : ((bool)searchQuery.descend))
                {
                    orderInterface = query.OrderByDescending(l => l.ExpenseTime);
                }
                else
                {
                    orderInterface = query.OrderBy(l => l.ExpenseTime);
                }
            }
            return orderInterface;
        }
    }
}
