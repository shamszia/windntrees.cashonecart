using DataAccessNET5.Models;
using DataAccessNET5.Models.Listing;
using System.Collections.Generic;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Inventory
{
    public class ReportRepository : EntityRepository<SummaryReportItem>
    {
        public ReportRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public StockTransactionRepository GetStockTransactionRepository()
        {
            return new StockTransactionRepository(new ApplicationContext());
        }

        public SalesTransactionRepository GetSalesTransactionRepository()
        {
            return new SalesTransactionRepository(new ApplicationContext());
        }

        public ExpenseRepository GetExpenseRepository()
        {
            return new ExpenseRepository(new ApplicationContext(),"Place,ExpenseNavigation");
        }

        public override List<SummaryReportItem> List(SearchInput queryObject)
        {
            List<SummaryReportItem> list = new List<SummaryReportItem>();

            SearchInput searchInputStock = new SearchInput {
                ascends = queryObject.ascends,
                checks = queryObject.checks,
                descend = queryObject.descend,
                descends = queryObject.descends,
                enabled = queryObject.enabled,
                endtime = queryObject.endtime,
                key = queryObject.key,
                keys = queryObject.keys,
                keyword = queryObject.keyword,
                keywords = queryObject.keywords,
                options = queryObject.options,
                page = queryObject.page,
                size = queryObject.size,
                source = queryObject.source,
                starttime = queryObject.starttime,
                total = queryObject.total
            };

            SearchInput searchInputSales = new SearchInput
            {
                ascends = queryObject.ascends,
                checks = queryObject.checks,
                descend = queryObject.descend,
                descends = queryObject.descends,
                enabled = queryObject.enabled,
                endtime = queryObject.endtime,
                key = queryObject.key,
                keys = queryObject.keys,
                keyword = queryObject.keyword,
                keywords = queryObject.keywords,
                options = queryObject.options,
                page = queryObject.page,
                size = queryObject.size,
                source = queryObject.source,
                starttime = queryObject.starttime,
                total = queryObject.total
            };

            SearchInput searchInputExpense = new SearchInput
            {
                ascends = queryObject.ascends,
                checks = queryObject.checks,
                descend = queryObject.descend,
                descends = queryObject.descends,
                enabled = queryObject.enabled,
                endtime = queryObject.endtime,
                key = queryObject.key,
                keys = queryObject.keys,
                keyword = queryObject.keyword,
                keywords = queryObject.keywords,
                options = queryObject.options,
                page = queryObject.page,
                size = queryObject.size,
                source = queryObject.source,
                starttime = queryObject.starttime,
                total = queryObject.total
            };

            List<DataAccessNET5.Models.StockTransaction> stockTransactions = GetStockTransactionRepository().GetStockTransactions(searchInputStock);
            List<DataAccessNET5.Models.SalesTransaction> salesTransactions = GetSalesTransactionRepository().GetSalesTransactions(searchInputSales);
            List<DataAccessNET5.Models.Expense> expenses = GetExpenseRepository().List(searchInputExpense);

            foreach (var stockTransaction in stockTransactions)
            {
                list.Add(new SummaryReportItem
                {
                    SummaryTime = stockTransaction.TransactionTime,
                    ReferenceNumber = stockTransaction.RefNumber,
                    Account = "1. Purchase",
                    Title = stockTransaction.LegalName,
                    Place = "",
                    Amount = stockTransaction.TotalPurchase - (stockTransaction.DiscountAmount == null ? 0 : ((decimal)stockTransaction.DiscountAmount))
                });
            }

            foreach (var saleTransaction in salesTransactions)
            {
                list.Add(new SummaryReportItem
                {
                    SummaryTime = saleTransaction.TransactionTime,
                    ReferenceNumber = saleTransaction.RefNumber,
                    Account = "2. Sale",
                    Title = saleTransaction.LegalName,
                    Place = "",
                    Amount = saleTransaction.TotalSales - (saleTransaction.DiscountAmount == null ? 0 : ((decimal)saleTransaction.DiscountAmount))
                });
            }

            foreach (var expense in expenses)
            {
                list.Add(new SummaryReportItem
                {
                    SummaryTime = expense.ExpenseTime,
                    ReferenceNumber = null,
                    Account = "3. Expense",
                    Title = expense.ExpenseNavigation.Name,
                    Place = expense.Place.Name,
                    Amount = expense.Amount
                });
            }

            return list;
        }
    }
}
