using DataAccessNET5.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using WindnTrees.CRUDS.Repository.Core;
using Microsoft.EntityFrameworkCore;
using WindnTrees.ICRUDS.Standard;
using Microsoft.Data.SqlClient;

namespace DataAccessNET5.Repositories.Inventory
{
    /// <summary>
    /// StockTransaction repository.
    /// </summary>
    public class StockTransactionRepository : EntityRepository<StockTransaction>
    {
        public StockTransactionRepository(string connectionString)
            : base(new ApplicationContext(connectionString), "read:StockPurchaseds,StockPayments")
        {
            
        }

        public StockTransactionRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override StockTransaction GenerateNewKey(StockTransaction contentObject)
        {
            DateTime transactionTime = DateTime.UtcNow;

            contentObject.TransactionTime = transactionTime;
            contentObject.Uid = Guid.NewGuid();
            
            foreach(var stockPayment in contentObject.StockPayments)
            {
                stockPayment.Uid = Guid.NewGuid();
                stockPayment.TransactionId = contentObject.Uid;
                stockPayment.PaymentTime = transactionTime;
            }

            foreach(var stockPurchased in contentObject.StockPurchaseds)
            {
                stockPurchased.Uid = Guid.NewGuid();
                stockPurchased.StockId = contentObject.Uid;
                stockPurchased.StockTime = transactionTime;
            }

            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        public override StockTransaction Read(object id)
        {
            try
            {
                Guid.Parse(id.ToString());
                return base.Read(id);
            }
            catch { }

            var longId = long.Parse(id.ToString());
            
            IQueryable<StockTransaction> queryInterface = entitySet;
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

        protected override IQueryable<StockTransaction> QueryRecords(IQueryable<StockTransaction> query, SearchInput searchQuery = null)
        {
            SearchField activeOption = null;
            SearchField creditOption = null;
            Expression<Func<StockTransaction, bool>> condition = null;

            if (searchQuery.starttime >= new DateTime(1800, 1, 1) && searchQuery.endtime >= new DateTime(1800, 1, 1))
            {
                condition = l => (l.TransactionTime >= searchQuery.starttime.Value.ToUniversalTime() && l.TransactionTime < searchQuery.endtime.Value.ToUniversalTime());
                query = query.Where(condition);
            }

            if (!string.IsNullOrEmpty(searchQuery.key))
            {
                Guid? companyID = null;
                try
                {
                    companyID = Guid.Parse(searchQuery.key);
                }
                catch { }

                condition = l => (l.SupplierId == companyID);
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

            if (searchQuery.enabled != null)
            {
                var active = (bool)searchQuery.enabled;

                condition = l => (l.Active == active);
                query = query.Where(condition);
            }

            searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;
            condition = l => (l.RefName.Contains(searchQuery.keyword) || l.Note.Contains(searchQuery.keyword) || l.RefNumber.ToString().Contains(searchQuery.keyword));
            query = query.Where(condition);

            return query;
        }

        protected override IOrderedQueryable<StockTransaction> SortRecords(IQueryable<StockTransaction> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<StockTransaction> orderInterface = null;
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

        public override StockTransaction Create(StockTransaction contentObject)
        {
            try
            {
                if (contentObject.Supplier != null)
                {   
                    //00000000-0000-0000-0000-000000000000
                    if (contentObject.Supplier.Uid.ToString().Equals("00000000-0000-0000-0000-000000000000"))
                    {
                        contentObject.Supplier.Uid = Guid.NewGuid();
                        contentObject.SupplierId = contentObject.Supplier.Uid;
                    }
                    else
                    {
                        contentObject.Supplier = null;
                    }
                }

                return base.Create(contentObject);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public override StockTransaction Update(StockTransaction contentObject)
        {
            try
            {
                DateTime transactionTime = DateTime.UtcNow;

                contentObject.TransactionTime = transactionTime;
                contentObject.Uid = Guid.NewGuid();

                if (contentObject.Supplier != null)
                {
                    if (contentObject.Supplier.Uid.ToString().Equals("00000000-0000-0000-0000-000000000000"))
                    {
                        contentObject.Supplier.Uid = Guid.NewGuid();
                        contentObject.SupplierId = contentObject.Supplier.Uid;
                    }
                    else
                    {
                        contentObject.Supplier = null;
                    }
                }

                foreach (var stockPayment in contentObject.StockPayments)
                {
                    stockPayment.Uid = Guid.NewGuid();
                    stockPayment.TransactionId = contentObject.Uid;
                    stockPayment.PaymentTime = transactionTime;
                }

                foreach (var stockPurchased in contentObject.StockPurchaseds)
                {
                    stockPurchased.Uid = Guid.NewGuid();
                    stockPurchased.StockId = contentObject.Uid;
                    stockPurchased.StockTime = transactionTime;
                }

                StockTransaction entityObject = entitySet.Add(contentObject).Entity;

                if (context.SaveChanges() > 0)
                {
                    entityObject = PostCreate(entityObject);
                    return entityObject;
                }

                return null;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        #region CancelStockTransaction
        public int CancelStockTransaction(string transactionID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string query = string.Format("UPDATE StockTransaction SET Active = 0 WHERE Uid = @Uid");

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

        #region DeleteStockTransaction
        public int DeleteStockTransaction(string transactionID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string deleteQuery = string.Format("DELETE FROM StockTransaction WHERE Uid = @Uid");

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

        #region DeleteStockPurchsed
        public int DeleteStockPurchsed(string stockID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string deleteQuery = string.Format("DELETE FROM StockPurchased WHERE StockID = @StockID");

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

        #region DeleteStockPayments
        public int DeleteStockPayments(string transactionID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string deleteQuery = string.Format("DELETE FROM StockPayment WHERE TransactionID = @TransactionID");

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
        public StockTransaction ExtractContentFromReader(SqlDataReader dataReader)
        {
            try
            {
                if (dataReader != null)
                {
                    //[Uid],[TransactionTime],[RefName],[RefNumber],[SupplierID],[Note],[Address],[Active],[UserID],[RowVersion],TotalPurchase

                    var content = new StockTransaction
                    {
                        Uid = Guid.Parse(dataReader["Uid"].ToString()),
                        TransactionTime = Convert.ToDateTime(dataReader["TransactionTime"]),
                        RefName = dataReader["RefName"] == DBNull.Value ? "" : Convert.ToString(dataReader["RefName"]),
                        RefNumber = Convert.ToInt64(dataReader["RefNumber"]),
                        LinkTransaction = (dataReader["LinkTransaction"] == DBNull.Value ? 0 : Convert.ToInt64(dataReader["LinkTransaction"])),
                        LegalName = dataReader["LegalName"] == DBNull.Value ? "" : Convert.ToString(dataReader["LegalName"]),
                        SupplierId = Guid.Parse(dataReader["SupplierID"].ToString()),
                        Note = dataReader["Note"] == DBNull.Value ? "" : Convert.ToString(dataReader["Note"]),
                        Address = dataReader["Address"] == DBNull.Value ? "" : Convert.ToString(dataReader["Address"]),
                        Active = dataReader["Active"] == DBNull.Value ? false : Convert.ToBoolean(dataReader["Active"]),
                        Credit = dataReader["Credit"] == DBNull.Value ? false : Convert.ToBoolean(dataReader["Credit"]),
                        DiscountAmount = dataReader["DiscountAmount"] == DBNull.Value ? 0 : Convert.ToInt64(dataReader["DiscountAmount"]),
                        UserId = dataReader["UserID"] == DBNull.Value ? "" : Convert.ToString(dataReader["UserID"]),
                        TotalPurchase = dataReader["TotalPurchase"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["TotalPurchase"]),
                        TotalPayments = dataReader["TotalPayments"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["TotalPayments"]),
                        RowVersion = (dataReader["RowVersion"] == DBNull.Value) ? null : ((byte[])dataReader["RowVersion"])
                    };

                    content.DiscountTotal = content.TotalPurchase - ((decimal)content.DiscountAmount);

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

        #region GetStockTransactionCount
        public int GetStockTransactionCount(SearchInput searchInput)
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

                string query = string.Format("SELECT COUNT(0) FROM [StockTransaction] LEFT OUTER JOIN Company ON ([StockTransaction].SupplierID = Company.Uid) WHERE (CAST([RefNumber] AS NVARCHAR(20)) LIKE @RefNumber OR [RefName] LIKE @RefName OR Company.LegalName LIKE @Name) {0};", queryPart);

                if (searchInput.starttime >= new DateTime(1800, 1, 1) && searchInput.endtime >= new DateTime(1800, 1, 1))
                {
                    query = string.Format("SELECT COUNT(0) FROM [StockTransaction] LEFT OUTER JOIN Company ON ([StockTransaction].SupplierID = Company.Uid) WHERE (CAST([RefNumber] AS NVARCHAR(20)) LIKE @RefNumber OR [RefName] LIKE @RefName OR Company.LegalName LIKE @Name) AND [TransactionTime] >= @StartTime AND [TransactionTime] <= @EndTime {0};", queryPart);
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
                    command.Parameters.Add(new SqlParameter("@Active", !(bool.Parse(activeOption.value))));
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

        #region GetStockTransactions
        public List<StockTransaction> GetStockTransactions(SearchInput searchInput)
        {
            SqlConnection sqlConnection = null;
            SqlDataReader dataReader = null;
            List<StockTransaction> transactions = new List<StockTransaction>();

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
                        //if canceled, i.e. active is false
                        queryPart = "AND Active = @Active";
                    }

                    creditOption = ((List<SearchField>)searchInput.options).Where(l => l.field == "Credit").SingleOrDefault();
                    if (creditOption != null)
                    {
                        queryPart = string.Format("{0} {1}", queryPart, "AND Credit = @Credit").Trim();
                    }
                }

                string query = string.Format("SELECT RNUMBER,[Uid],[TransactionTime],[RefName],[RefNumber],[LinkTransaction],[SupplierID],LegalName,[Note],[Address],TotalPurchase,TotalPayments,[Credit],[DiscountAmount],[Active],[UserID],[RowVersion] FROM	(SELECT ROW_NUMBER() OVER (ORDER BY [RefNumber] DESC) AS RNUMBER,StockTransaction.[Uid],[TransactionTime],[RefName],[RefNumber],[LinkTransaction],[Company].LegalName,[SupplierID],[Note],StockTransaction.[Address],[Credit],[DiscountAmount],[Active],StockTransaction.[UserID],StockTransaction.[RowVersion],TotalPurchase = (SELECT SUM(Quantity * UnitPrice) FROM StockPurchased WHERE StockID = [StockTransaction].[Uid]), TotalPayments = (SELECT SUM(Amount) FROM StockPayment WHERE TransactionID = [StockTransaction].[Uid]) FROM [dbo].[StockTransaction] LEFT OUTER JOIN Company ON ([StockTransaction].SupplierID = Company.Uid) WHERE (CAST([RefNumber] AS NVARCHAR(20)) LIKE @RefNumber OR [RefName] LIKE @RefName OR Company.LegalName LIKE @Name) {0}) Transactions WHERE RNUMBER BETWEEN (((@PAGENUMBER - 1) * @PAGESIZE)+1) AND (@PAGENUMBER * @PAGESIZE) ORDER BY RefNumber DESC, LinkTransaction DESC;", queryPart);

                if (size > 0 && page > 0)
                {
                    if (searchInput.starttime >= new DateTime(1800, 1, 1) && searchInput.endtime >= new DateTime(1800, 1, 1))
                    {
                        query = string.Format("SELECT RNUMBER,[Uid],[TransactionTime],[RefName],[RefNumber],[LinkTransaction],[SupplierID],LegalName,[Note],[Address],TotalPurchase,TotalPayments,[Credit],[DiscountAmount],[Active],[UserID],[RowVersion] FROM   (SELECT ROW_NUMBER() OVER (ORDER BY [RefNumber] DESC) AS RNUMBER, StockTransaction.[Uid],[TransactionTime],[RefName],[RefNumber],[LinkTransaction],[Company].LegalName,[SupplierID],[Note],StockTransaction.[Address],[Credit],[DiscountAmount],[Active],StockTransaction.[UserID],StockTransaction.[RowVersion],TotalPurchase = (SELECT SUM(Quantity * UnitPrice) FROM StockPurchased WHERE StockID = [StockTransaction].[Uid]), TotalPayments = (SELECT SUM(Amount) FROM StockPayment WHERE TransactionID = [StockTransaction].[Uid]) FROM [dbo].[StockTransaction] LEFT OUTER JOIN Company ON ([StockTransaction].SupplierID = Company.Uid) WHERE (CAST([RefNumber] AS NVARCHAR(20)) LIKE @RefNumber OR [RefName] LIKE @RefName OR Company.LegalName LIKE @Name) AND [TransactionTime] >= @StartTime AND [TransactionTime] <= @EndTime {0}) Transactions WHERE RNUMBER BETWEEN (((@PAGENUMBER - 1) * @PAGESIZE)+1) AND (@PAGENUMBER * @PAGESIZE) ORDER BY RefNumber DESC, LinkTransaction DESC;", queryPart);
                    }
                }
                else
                {
                    query = string.Format("SELECT StockTransaction.[Uid],[TransactionTime],[RefName],[RefNumber],[LinkTransaction],[Company].LegalName,[SupplierID],[Note],StockTransaction.[Address],[Credit],[DiscountAmount],[Active],StockTransaction.[UserID],StockTransaction.[RowVersion],TotalPurchase = (SELECT SUM(Quantity * UnitPrice) FROM StockPurchased WHERE StockID = [StockTransaction].[Uid]),TotalPayments = (SELECT SUM(Amount) FROM StockPayment WHERE TransactionID = [StockTransaction].[Uid]) FROM [dbo].[StockTransaction] LEFT OUTER JOIN Company ON ([StockTransaction].SupplierID = Company.Uid) WHERE (CAST([RefNumber] AS NVARCHAR(20)) LIKE @RefNumber OR [RefName] LIKE @RefName OR Company.LegalName LIKE @Name) AND [TransactionTime] >= @StartTime AND [TransactionTime] <= @EndTime {0} ORDER BY [RefNumber], LinkTransaction DESC", queryPart);
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

        #region ListTransactionsWithStockAndPayments
        public List<StockTransaction> ListTransactionsWithStockAndPayments(SearchInput queryObject)
        {
            IQueryable<StockTransaction> queryInterface = entitySet;
            queryInterface = QueryListRecords(queryInterface, queryObject);

            foreach (var relatedObject in "Company,StockPayments,StockPurchaseds".Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                queryInterface = queryInterface.Include(relatedObject);
            }

            IOrderedQueryable<StockTransaction> orderInterface = SortListRecords(queryInterface, queryObject);

            List<StockTransaction> stockTransactions = orderInterface.ToList();

            foreach (var stockTransaction in stockTransactions)
            {
                if (stockTransaction.Supplier != null)
                {
                    stockTransaction.LegalName = stockTransaction.Supplier.LegalName;
                    stockTransaction.Supplier = null;
                }
            }

            return stockTransactions;
        }
        #endregion
    }
}