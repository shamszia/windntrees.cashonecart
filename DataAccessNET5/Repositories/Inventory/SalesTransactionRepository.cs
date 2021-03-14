using DataAccessNET5.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Inventory
{
    /// <summary>
    /// SalesTransaction repository.
    /// </summary>
    public class SalesTransactionRepository : EntityRepository<SalesTransaction>
    {
        public SalesTransactionRepository(string connectionString)
            : base(new ApplicationContext(connectionString), "read:SalesPayments,StockSolds")
        {

        }

        public SalesTransactionRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override SalesTransaction GenerateNewKey(SalesTransaction contentObject)
        {
            DateTime transactionTime = DateTime.UtcNow;

            contentObject.TransactionTime = transactionTime;
            contentObject.Uid = Guid.NewGuid();

            foreach (var salesPayment in contentObject.SalesPayments)
            {
                salesPayment.Uid = Guid.NewGuid();
                salesPayment.TransactionId = contentObject.Uid;
                salesPayment.PaymentTime = transactionTime;
            }

            foreach (var stockSold in contentObject.StockSolds)
            {
                stockSold.Uid = Guid.NewGuid();
                stockSold.StockId = contentObject.Uid;
                stockSold.StockTime = transactionTime;
            }

            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        public override SalesTransaction Read(object id)
        {
            try
            {
                Guid.Parse(id.ToString());
                return base.Read(id);
            }
            catch { }

            var longId = long.Parse(id.ToString());

            IQueryable<SalesTransaction> queryInterface = entitySet;
            if (relatedObjects.IndexOf(":") > 0)
            {
                string[] actions_relatedObjects = relatedObjects.Split(new char[] { ':' });
                string[] actions = actions_relatedObjects[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string[] objects = actions_relatedObjects[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var action in actions)
                {
                    if (action.Equals("Read", StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var relatedObject in objects)
                        {
                            queryInterface = queryInterface.Include(relatedObject);
                        }
                    }
                }
            }

            var result = queryInterface.Where(l => (l.RefNumber == longId)).SingleOrDefault();

            return result;
        }

        protected override IQueryable<SalesTransaction> QueryRecords(IQueryable<SalesTransaction> query, SearchInput searchQuery = null)
        {
            SearchField activeOption = null;
            SearchField creditOption = null;
            Expression<Func<SalesTransaction, bool>> condition = null;

            if (searchQuery.starttime >= new DateTime(1800, 1, 1) && searchQuery.endtime >= new DateTime(1800, 1, 1))
            {
                condition = l => (l.TransactionTime >= searchQuery.starttime.Value.ToUniversalTime() && l.TransactionTime < searchQuery.endtime.Value.ToUniversalTime());
                query = query.Where(condition);
            }

            if (((List<SearchField>)searchQuery.options) != null)
            {
                activeOption = ((List<SearchField>)searchQuery.options).Where(l => l.field == "Active").SingleOrDefault();
                if (activeOption != null)
                {
                    bool active = bool.Parse(activeOption.value);
                    //canceled / returned = true otherwise is false 
                    //i.e active = true is returned (or canceled)
                    condition = l => (l.Active == (!active));
                    query = query.Where(condition);
                }

                creditOption = ((List<SearchField>)searchQuery.options).Where(l => l.field == "Credit").SingleOrDefault();
                if (creditOption != null)
                {
                    bool credit = bool.Parse(creditOption.value);
                    condition = l => (l.Credit == credit);
                    query = query.Where(condition);
                }
            }

            searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;
            condition = l => (l.RefName.Contains(searchQuery.keyword) || l.Note.Contains(searchQuery.keyword) || l.RefNumber.ToString().Contains(searchQuery.keyword));
            query = query.Where(condition);

            return query;
        }

        protected override IOrderedQueryable<SalesTransaction> SortRecords(IQueryable<SalesTransaction> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<SalesTransaction> orderInterface = null;
            if (searchQuery != null)
            {
                if (searchQuery.descend == null ? false : ((bool)searchQuery.descend))
                {
                    orderInterface = query.OrderByDescending(l => l.RefNumber).OrderByDescending(l => l.LinkTransaction);
                }
                else
                {
                    orderInterface = query.OrderBy(l => l.RefNumber).OrderByDescending(l => l.LinkTransaction);
                }
            }
            return orderInterface;
        }

        public override SalesTransaction Create(SalesTransaction contentObject)
        {
            try
            {
                if (contentObject.Buyer != null)
                {
                    //00000000-0000-0000-0000-000000000000
                    if (contentObject.Buyer.Uid.ToString().Equals("00000000-0000-0000-0000-000000000000"))
                    {
                        contentObject.Buyer.Uid = Guid.NewGuid();
                        contentObject.BuyerId = contentObject.Buyer.Uid;
                    }
                    else
                    {
                        contentObject.Buyer = null;
                    }
                }

                return base.Create(contentObject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override SalesTransaction Update(SalesTransaction contentObject)
        {
            try
            {
                DateTime transactionTime = DateTime.UtcNow;

                contentObject.TransactionTime = transactionTime;
                contentObject.Uid = Guid.NewGuid();

                if (contentObject.Buyer != null)
                {
                    if (contentObject.Buyer.Uid.ToString().Equals("00000000-0000-0000-0000-000000000000"))
                    {
                        contentObject.Buyer.Uid = Guid.NewGuid();
                        contentObject.BuyerId = contentObject.Buyer.Uid;
                    }
                    else
                    {
                        contentObject.Buyer = null;
                    }
                }

                foreach (var salesPayment in contentObject.SalesPayments)
                {
                    salesPayment.Uid = Guid.NewGuid();
                    salesPayment.TransactionId = contentObject.Uid;
                    salesPayment.PaymentTime = transactionTime;
                }

                foreach (var stockSold in contentObject.StockSolds)
                {
                    stockSold.Uid = Guid.NewGuid();
                    stockSold.StockId = contentObject.Uid;
                    stockSold.StockTime = transactionTime;
                }

                SalesTransaction entityObject = entitySet.Add(contentObject).Entity;

                if (context.SaveChanges() > 0)
                {
                    entityObject = PostCreate(entityObject);
                    return entityObject;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region CancelSalesTransaction
        public int CancelSalesTransaction(string transactionID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string query = string.Format("UPDATE SalesTransaction SET Active = 0 WHERE Uid = @Uid");

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.Add(new SqlParameter("@Uid", transactionID));

                command.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    return command.ExecuteNonQuery();
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    try
                    {
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region DeleteSalesTransaction
        public int DeleteSalesTransaction(string transactionID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string deleteQuery = string.Format("DELETE FROM SalesTransaction WHERE Uid = @Uid");

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection);

                deleteCommand.Parameters.Add(new SqlParameter("@Uid", transactionID));

                deleteCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    return deleteCommand.ExecuteNonQuery();
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    try
                    {
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region DeleteStockSold
        public int DeleteStockSold(string stockID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string deleteQuery = string.Format("DELETE FROM StockSold WHERE StockID = @StockID");

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection);

                deleteCommand.Parameters.Add(new SqlParameter("@StockID", stockID));

                deleteCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    return deleteCommand.ExecuteNonQuery();
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    try
                    {
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region DeleteSalesPayments
        public int DeleteSalesPayments(string transactionID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string deleteQuery = string.Format("DELETE FROM SalesPayment WHERE TransactionID = @TransactionID");

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection);

                deleteCommand.Parameters.Add(new SqlParameter("@TransactionID", transactionID));

                deleteCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    return deleteCommand.ExecuteNonQuery();
                }
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    try
                    {
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region ExtractContentFromReader
        public SalesTransaction ExtractContentFromReader(SqlDataReader dataReader)
        {
            try
            {
                if (dataReader != null)
                {
                    var content = new SalesTransaction
                    {
                        Uid = Guid.Parse(dataReader["Uid"].ToString()),
                        TransactionTime = Convert.ToDateTime(dataReader["TransactionTime"]),
                        RefName = dataReader["RefName"] == DBNull.Value ? "" : Convert.ToString(dataReader["RefName"]),
                        LegalName = dataReader["LegalName"] == DBNull.Value ? "" : Convert.ToString(dataReader["LegalName"]),
                        RefNumber = Convert.ToInt64(dataReader["RefNumber"]),
                        LinkTransaction = (dataReader["LinkTransaction"] == DBNull.Value ? 0 : Convert.ToInt64(dataReader["LinkTransaction"])),
                        BuyerId = Guid.Parse(dataReader["BuyerID"].ToString()),
                        Note = dataReader["Note"] == DBNull.Value ? "" : Convert.ToString(dataReader["Note"]),
                        Address = dataReader["Address"] == DBNull.Value ? "" : Convert.ToString(dataReader["Address"]),
                        Active = dataReader["Active"] == DBNull.Value ? false : Convert.ToBoolean(dataReader["Active"]),
                        Credit = dataReader["Credit"] == DBNull.Value ? false : Convert.ToBoolean(dataReader["Credit"]),
                        DiscountAmount = dataReader["DiscountAmount"] == DBNull.Value ? 0 : Convert.ToInt64(dataReader["DiscountAmount"]),
                        UserId = dataReader["UserID"] == DBNull.Value ? "" : Convert.ToString(dataReader["UserID"]),
                        TotalSales = dataReader["TotalSales"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["TotalSales"]),
                        TotalPayments = dataReader["TotalPayments"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["TotalPayments"]),
                        RowVersion = (dataReader["RowVersion"] == DBNull.Value) ? null : ((byte[])dataReader["RowVersion"])
                    };

                    content.DiscountTotal = content.TotalSales - ((decimal)content.DiscountAmount);

                    return content;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetSalesTransactionCount
        public int GetSalesTransactionCount(SearchInput searchInput)
        {
            SqlConnection sqlConnection = null;
            int stockCount = 0;

            searchInput.keyword = string.IsNullOrEmpty(searchInput.keyword) ? "%" : string.Format("%{0}%", searchInput.keyword);

            try
            {
                string queryPart = string.Empty;
                SearchField activeOption = null;
                SearchField creditOption = null;

                if (searchInput.options != null)
                {
                    activeOption = ((List<SearchField>)searchInput.options).Where(l => l.field == "Active").SingleOrDefault();
                    if (activeOption != null)
                    {
                        //if canceled, i.e active is false
                        queryPart = "AND Active = @Active";
                    }

                    creditOption = ((List<SearchField>)searchInput.options).Where(l => l.field == "Credit").SingleOrDefault();
                    if (creditOption != null)
                    {
                        queryPart = string.Format("{0} {1}", queryPart, "AND Credit = @Credit").Trim();
                    }
                }

                string query = string.Format("SELECT COUNT(0) FROM [SalesTransaction] LEFT OUTER JOIN Company ON ([SalesTransaction].BuyerID = Company.Uid) WHERE (CAST([RefNumber] AS NVARCHAR(10)) LIKE @RefNumber OR [RefName] LIKE @RefName OR Company.LegalName LIKE @Name) {0};", queryPart);

                if (searchInput.starttime >= new DateTime(1800, 1, 1) && searchInput.endtime >= new DateTime(1800, 1, 1))
                {
                    query = string.Format("SELECT COUNT(0) FROM [SalesTransaction] LEFT OUTER JOIN Company ON ([SalesTransaction].BuyerID = Company.Uid) WHERE (CAST([RefNumber] AS NVARCHAR(10)) LIKE @RefNumber OR [RefName] LIKE @RefName OR Company.LegalName LIKE @Name) AND [TransactionTime] >= @StartTime AND [TransactionTime] <= @EndTime {0};", queryPart);
                }

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.Add(new SqlParameter("@RefNumber", searchInput.keyword));
                command.Parameters.Add(new SqlParameter("@RefName", searchInput.keyword));
                command.Parameters.Add(new SqlParameter("@Name", searchInput.keyword));

                if (searchInput.starttime >= new DateTime(1800, 1, 1) && searchInput.endtime >= new DateTime(1800, 1, 1))
                {
                    command.Parameters.Add(new SqlParameter("@StartTime", searchInput.starttime.Value.ToUniversalTime()));
                    command.Parameters.Add(new SqlParameter("@EndTime", searchInput.endtime.Value.ToUniversalTime()));
                }

                if (activeOption != null)
                {
                    //if canceled, i.e active is false
                    command.Parameters.Add(new SqlParameter("@Active", !bool.Parse(activeOption.value)));
                }

                if (creditOption != null)
                {
                    command.Parameters.Add(new SqlParameter("@Credit", bool.Parse(creditOption.value)));
                }

                command.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    stockCount = (int)command.ExecuteScalar();
                }

                return stockCount;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (sqlConnection != null)
                {
                    try
                    {
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        #endregion
        
        #region GetSalesTransactions
        public List<SalesTransaction> GetSalesTransactions(SearchInput searchInput)
        {
            SqlConnection sqlConnection = null;
            SqlDataReader dataReader = null;
            List<SalesTransaction> transactions = new List<SalesTransaction>();

            searchInput.keyword = string.IsNullOrEmpty(searchInput.keyword) ? "%" : string.Format("%{0}%", searchInput.keyword);

            try
            {
                string queryPart = string.Empty;
                SearchField activeOption = null;
                SearchField creditOption = null;
                int size = searchInput.size == null ? 0 : ((int)searchInput.size);
                int page = searchInput.page == null ? 0 : ((int)searchInput.page);

                if (searchInput.options != null)
                {
                    activeOption = ((List<SearchField>)searchInput.options).Where(l => l.field == "Active").SingleOrDefault();
                    if (activeOption != null)
                    {
                        //if canceled, i.e active is false
                        queryPart = "AND Active = @Active";
                    }

                    creditOption = ((List<SearchField>)searchInput.options).Where(l => l.field == "Credit").SingleOrDefault();
                    if (creditOption != null)
                    {
                        queryPart = string.Format("{0} {1}", queryPart, "AND Credit = @Credit").Trim();
                    }
                }

                string query = string.Format("SELECT RNUMBER,[Uid],[TransactionTime],[RefName],[RefNumber],[LinkTransaction],[BuyerID],LegalName,[Note],[Address],[Credit],[DiscountAmount],TotalSales,TotalPayments,[Active],[UserID],[RowVersion] FROM (SELECT ROW_NUMBER() OVER (ORDER BY [RefNumber] DESC) AS RNUMBER, [SalesTransaction].[Uid],[TransactionTime],[RefName],[RefNumber],[LinkTransaction],[BuyerID],Company.LegalName,[Note],[SalesTransaction].[Address],[Credit],[DiscountAmount],[Active],[SalesTransaction].[UserID],[SalesTransaction].[RowVersion],TotalSales = (SELECT SUM(Quantity * SalesPrice) FROM StockSold WHERE StockID = [SalesTransaction].[Uid]),TotalPayments = (SELECT SUM(Amount) FROM SalesPayment WHERE TransactionID = [SalesTransaction].[Uid]) FROM [SalesTransaction] LEFT OUTER JOIN Company ON ([SalesTransaction].[BuyerID] = Company.Uid) WHERE (CAST([RefNumber] AS NVARCHAR(10)) LIKE @RefNumber OR [RefName] LIKE @RefName OR Company.LegalName LIKE @Name) {0}) Transactions WHERE RNUMBER BETWEEN (((@PAGENUMBER - 1) * @PAGESIZE)+1) AND (@PAGENUMBER * @PAGESIZE) ORDER BY RefNumber DESC, LinkTransaction DESC;", queryPart);

                if (size > 0 && page > 0)
                {
                    if (searchInput.starttime >= new DateTime(1800, 1, 1) && searchInput.endtime >= new DateTime(1800, 1, 1))
                    {
                        query = string.Format("SELECT RNUMBER,[Uid],[TransactionTime],[RefName],[RefNumber],[LinkTransaction],[BuyerID],LegalName,[Note],[Address],[Credit],[DiscountAmount],TotalSales,TotalPayments,[Active],[UserID],[RowVersion] FROM (SELECT ROW_NUMBER() OVER (ORDER BY [RefNumber] DESC) AS RNUMBER, [SalesTransaction].[Uid],[TransactionTime],[RefName],[RefNumber],[LinkTransaction],[BuyerID],Company.LegalName,[Note],[SalesTransaction].[Address],[Credit],[DiscountAmount],[Active],[SalesTransaction].[UserID],[SalesTransaction].[RowVersion],TotalSales = (SELECT SUM(Quantity * SalesPrice) FROM StockSold WHERE StockID = [SalesTransaction].[Uid]),TotalPayments = (SELECT SUM(Amount) FROM SalesPayment WHERE TransactionID = [SalesTransaction].[Uid]) FROM [SalesTransaction] LEFT OUTER JOIN Company ON ([SalesTransaction].[BuyerID] = Company.Uid) WHERE (CAST([RefNumber] AS NVARCHAR(10)) LIKE @RefNumber OR [RefName] LIKE @RefName OR Company.LegalName LIKE @Name) AND [TransactionTime] >= @StartTime AND [TransactionTime] <= @EndTime {0}) Transactions WHERE RNUMBER BETWEEN (((@PAGENUMBER - 1) * @PAGESIZE)+1) AND (@PAGENUMBER * @PAGESIZE) ORDER BY RefNumber DESC, LinkTransaction DESC;", queryPart);
                    }
                }
                else
                {
                    query = string.Format("SELECT [SalesTransaction].[Uid],[TransactionTime],[RefName],[RefNumber],[LinkTransaction],[BuyerID],Company.LegalName,[Note],[SalesTransaction].[Address],[Credit],[DiscountAmount],[Active],[SalesTransaction].[UserID],[SalesTransaction].[RowVersion],TotalSales = (SELECT SUM(Quantity * SalesPrice) FROM StockSold WHERE StockID = [SalesTransaction].[Uid]),TotalPayments = (SELECT SUM(Amount) FROM SalesPayment WHERE TransactionID = [SalesTransaction].[Uid]) FROM [SalesTransaction] LEFT OUTER JOIN Company ON ([SalesTransaction].[BuyerID] = Company.Uid) WHERE (CAST([RefNumber] AS NVARCHAR(10)) LIKE @RefNumber OR [RefName] LIKE @RefName OR Company.LegalName LIKE @Name) AND [TransactionTime] >= @StartTime AND [TransactionTime] <= @EndTime {0} ORDER BY [RefNumber];", queryPart);
                }

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.Add(new SqlParameter("@RefNumber", searchInput.keyword));
                command.Parameters.Add(new SqlParameter("@RefName", searchInput.keyword));
                command.Parameters.Add(new SqlParameter("@Name", searchInput.keyword));

                if (searchInput.starttime >= new DateTime(1800, 1, 1) && searchInput.endtime >= new DateTime(1800, 1, 1))
                {
                    command.Parameters.Add(new SqlParameter("@StartTime", searchInput.starttime.Value.ToUniversalTime()));
                    command.Parameters.Add(new SqlParameter("@EndTime", searchInput.endtime.Value.ToUniversalTime()));
                }

                if (activeOption != null)
                {
                    command.Parameters.Add(new SqlParameter("@Active", !bool.Parse(activeOption.value)));
                }

                if (creditOption != null)
                {
                    command.Parameters.Add(new SqlParameter("@Credit", bool.Parse(creditOption.value)));
                }

                if (size > 0 && page > 0)
                {
                    command.Parameters.Add(new SqlParameter("@PAGENUMBER", searchInput.page));
                    command.Parameters.Add(new SqlParameter("@PAGESIZE", searchInput.size));
                }

                command.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dataReader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            transactions.Add(ExtractContentFromReader(dataReader));
                        }
                    }
                }

                return transactions;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dataReader != null)
                {
                    try
                    {
                        dataReader.Close();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        #endregion

        #region ListTransactionsWithSalesAndPayments
        public List<SalesTransaction> ListTransactionsWithSalesAndPayments(SearchInput queryObject)
        {
            IQueryable<SalesTransaction> queryInterface = entitySet;
            queryInterface = QueryListRecords(queryInterface, queryObject);

            foreach (var relatedObject in "Company,SalesPayments,StockSolds".Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                queryInterface = queryInterface.Include(relatedObject);
            }

            IOrderedQueryable<SalesTransaction> orderInterface = SortListRecords(queryInterface, queryObject);

            List<SalesTransaction> salesTransactions = orderInterface.ToList();

            foreach (var salesTransaction in salesTransactions)
            {
                if (salesTransaction.Buyer != null)
                {
                    salesTransaction.LegalName = salesTransaction.Buyer.LegalName;
                    salesTransaction.Buyer = null;
                }
            }

            return salesTransactions;
        }
        #endregion
    }
}
