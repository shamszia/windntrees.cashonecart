using DataAccessNET5.Models;
using DataAccessNET5.Models.Listing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Inventory
{
    /// <summary>
    /// Stock repository.
    /// </summary>
    public class StockRepository : EntityRepository<Stock>
    {
        public StockRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public StockRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        public override List<Stock> List(SearchInput queryObject)
        {
            return GetStock(queryObject);
        }

        public Stock ExtractContentFromReader(SqlDataReader dataReader, bool stockTime = false)
        {
            try
            {
                if (dataReader != null)
                {
                    var content = new Stock
                    {
                        ProductId = Guid.Parse(dataReader["ProductID"].ToString()),
                        ProductName = Convert.ToString(dataReader["ProductName"]),
                        Account = Convert.ToString(dataReader["Account"]),
                        Quantity = Convert.ToInt32(dataReader["Quantity"]),
                        SalesPrice = Convert.ToDecimal(dataReader["SalesPrice"]),
                        UnitPrice = Convert.ToDecimal(dataReader["CalculatedPrice"])
                    };

                    if (stockTime)
                    {
                        content.StockTime = Convert.ToString(dataReader["StockTime"]);
                    }

                    return content;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Stock ExtractPlaceContentFromReader(SqlDataReader dataReader)
        {
            try
            {
                if (dataReader != null)
                {
                    var content = new Stock
                    {
                        //RNUMBER,[ProductID],ProductName,Account,Quantity,SalesPrice,UnitPrice
                        //StockTime,ProductID,ProductName,Account,Quantity,SalesPrice,UnitPrice
                        StockTime = Convert.ToString(dataReader["StockTime"]),
                        ProductId = Guid.Parse(dataReader["ProductID"].ToString()),
                        ProductName = Convert.ToString(dataReader["ProductName"]),
                        Account = Convert.ToString(dataReader["Account"]),
                        Quantity = Convert.ToInt32(dataReader["Quantity"]),
                        SalesPrice = Convert.ToDecimal(dataReader["SalesPrice"]),
                        UnitPrice = Convert.ToDecimal(dataReader["UnitPrice"])
                    };

                    if (dataReader["PlaceId"] != DBNull.Value)
                    {
                        content.PlaceId = Guid.Parse(dataReader["PlaceId"].ToString());
                    }

                    if (dataReader["PlaceName"] != DBNull.Value)
                    {
                        content.PlaceName = Convert.ToString(dataReader["PlaceName"]);
                    }

                    return content;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region GetStock
        /// <summary>
        /// Gets stock.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<Stock> GetStock(SearchInput searchQuery)
        {
            List<Stock> contents = new List<Stock>();
            SqlDataReader dReader = null;
            int totalRecords = 0;
            try
            {
                string orderBy = (searchQuery.descend == null) ? "" : (((bool)searchQuery.descend) ? "DESC" : "");
                string selectQuery = string.Empty;
                string countQuery = string.Empty;
                string stockOperator = " = ";

                if (((List<SearchField>)searchQuery.options) != null)
                {
                    var stockOption = ((List<SearchField>)searchQuery.options).Where(l => l.field == "inOutStock").SingleOrDefault();
                    if (stockOption != null)
                    {
                        if (stockOption.value.Equals("inStock"))
                        {
                            stockOperator = " <> ";
                        }
                        else if (stockOption.value.Equals("outOfStock"))
                        {
                            stockOperator = " = ";
                        }
                    }
                }
                
                selectQuery = string.Format("SELECT RNUMBER,[ProductID],ProductName,Account,Quantity,SalesPrice,CalculatedPrice FROM (SELECT ROW_NUMBER() OVER (ORDER BY [Product].Name {0}) AS RNUMBER,[ProductID], Quantity,[Product].Name AS ProductName,[Product].SalesPrice,[Product].Account,CalculatedPrice FROM (SELECT [ProductID], [Account], Quantity, StockValue, StockValue / (IIF(Quantity = 0,1,Quantity)) AS CalculatedPrice FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, SUM(StockValue) StockValue FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, StockValue = ISNULL((SELECT SUM(DrAmount) - SUM(CrAmount) AS StockValue FROM [Transaction] WHERE (StockQuantity.Account = AccountNo AND SubscriptionID = @SubscriptionID)),0) FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account], SUM([Quantity]) AS Quantity FROM [StockPurchased] LEFT OUTER JOIN Product ON (Product.Uid = [StockPurchased].ProductID) WHERE Product.SubscriptionID = @SubscriptionID GROUP BY [StockPurchased].[ProductID], [StockPurchased].[Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], ((-1) * SUM([Quantity])) AS Quantity FROM [StockSold] LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) WHERE Product.SubscriptionID = @SubscriptionID GROUP BY [StockSold].[ProductID], [StockSold].[Account]) StockQuantity GROUP BY [ProductID], [Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], 0 AS Quantity, ((-1) * (([StockSold].[Quantity] * [StockSold].[UnitPrice]) + ([StockSold].[Quantity]*[StockSold].[SalesTax]) + ([StockSold].[Quantity]*[StockSold].[IncomeTax]))) AS StockValue FROM StockSold LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND [StockSold].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'SalesTransaction') UNION SELECT [ProductID],[Account],0 AS [Quantity], (([Quantity] * [UnitPrice]) + ([Quantity] * [SalesTax]) + ([Quantity] * [IncomeTax])) AS StockValue FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account],SUM([Quantity]) AS [Quantity], AVG([StockPurchased].[UnitPrice]) AS [UnitPrice], AVG([StockPurchased].[SalesTax]) AS [SalesTax], AVG([StockPurchased].[IncomeTax]) AS [IncomeTax] FROM StockPurchased LEFT OUTER JOIN Product ON (Product.Uid = StockPurchased.ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND [StockPurchased].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'StockTransaction') GROUP BY [ProductID], [StockPurchased].[Account]) PurchasedStock) CorrectedStock GROUP BY [ProductID], [Account]) AvailableStock) CalculatedStock LEFT OUTER JOIN Product ON (Product.Uid = [ProductID]) WHERE [Product].Name LIKE @ProductName) AS Stock WHERE Quantity {1} 0 AND RNUMBER BETWEEN (((@PAGENUMBER - 1) * @PAGESIZE)+1) AND (@PAGENUMBER * @PAGESIZE) ORDER BY ProductName {0}", orderBy, stockOperator);
                countQuery = string.Format("SELECT COUNT(0) FROM (SELECT [ProductID], [Account], Quantity, StockValue, StockValue / (IIF(Quantity = 0,1,Quantity)) AS CalculatedPrice FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, SUM(StockValue) StockValue FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, StockValue = ISNULL((SELECT SUM(DrAmount) - SUM(CrAmount) AS StockValue FROM [Transaction] WHERE (StockQuantity.Account = AccountNo AND SubscriptionID = @SubscriptionID)),0) FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account], SUM([Quantity]) AS Quantity FROM [StockPurchased] LEFT OUTER JOIN Product ON (Product.Uid = [StockPurchased].ProductID) WHERE Product.SubscriptionID = @SubscriptionID GROUP BY [StockPurchased].[ProductID], [StockPurchased].[Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], ((-1) * SUM([Quantity])) AS Quantity FROM [StockSold] LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) WHERE Product.SubscriptionID = @SubscriptionID GROUP BY [StockSold].[ProductID], [StockSold].[Account]) StockQuantity GROUP BY [ProductID], [Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], 0 AS Quantity, ((-1) * (([StockSold].[Quantity] * [StockSold].[UnitPrice]) + ([StockSold].[Quantity]*[StockSold].[SalesTax]) + ([StockSold].[Quantity]*[StockSold].[IncomeTax]))) AS StockValue FROM StockSold LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND [StockSold].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'SalesTransaction') UNION SELECT [ProductID],[Account],0 AS [Quantity], (([Quantity] * [UnitPrice]) + ([Quantity] * [SalesTax]) + ([Quantity] * [IncomeTax])) AS StockValue FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account],SUM([Quantity]) AS [Quantity], AVG([StockPurchased].[UnitPrice]) AS [UnitPrice], AVG([StockPurchased].[SalesTax]) AS [SalesTax], AVG([StockPurchased].[IncomeTax]) AS [IncomeTax] FROM StockPurchased LEFT OUTER JOIN Product ON (Product.Uid = StockPurchased.ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND [StockPurchased].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'StockTransaction') GROUP BY [ProductID], [StockPurchased].[Account]) PurchasedStock) CorrectedStock GROUP BY [ProductID], [Account]) AvailableStock) CalculatedStock LEFT OUTER JOIN Product ON (Product.Uid = [ProductID]) WHERE Quantity {0} 0 AND [Product].Name LIKE @ProductName;", stockOperator);

                SqlConnection sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand countCommand = new SqlCommand(countQuery, sqlConnection);

                countCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));
                countCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
                selectCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));
                selectCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));

                selectCommand.Parameters.Add(new SqlParameter("@PAGENUMBER", searchQuery.page));
                selectCommand.Parameters.Add(new SqlParameter("@PAGESIZE", searchQuery.size));

                selectCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;
                countCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    totalRecords = Util.ConvertToInt32(countCommand.ExecuteScalar(), 0);

                    if (totalRecords > 0)
                    {
                        dReader = selectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dReader != null)
                        {
                            while (dReader.Read())
                            {
                                contents.Add(ExtractContentFromReader(dReader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dReader != null)
                {
                    try
                    {
                        dReader.Close();
                    }
                    catch { }
                }
            }
            return contents;
        }
        #endregion

        #region GetPurchaseStock
        /// <summary>
        /// Gets purchased stock.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<Stock> GetPurchaseStock(SearchInput searchQuery)
        { 
            List<Stock> contents = new List<Stock>();
            SqlDataReader dReader = null;
            int totalRecords = 0;
            try
            {
                string orderBy = (searchQuery.descend == null) ? "" : (((bool)searchQuery.descend) ? "DESC" : "");

                string selectQuery = string.Empty;
                string countQuery = string.Empty;

                selectQuery = string.Format("SELECT RNUMBER,[ProductID],ProductName,Account,Quantity,SalesPrice,CalculatedPrice FROM (SELECT ROW_NUMBER() OVER (ORDER BY [Product].Name {0}) AS RNUMBER,[ProductID], Quantity,[Product].Name AS ProductName,[Product].SalesPrice,[Product].Account,CalculatedPrice FROM (SELECT [ProductID], [Account], Quantity, StockValue, StockValue / (IIF(Quantity = 0,1,Quantity)) AS CalculatedPrice FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, SUM(StockValue) StockValue FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, StockValue = ISNULL((SELECT SUM(DrAmount) - SUM(CrAmount) AS StockValue FROM [Transaction] WHERE (StockQuantity.Account = AccountNo AND SubscriptionID = @SubscriptionID)),0) FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account], SUM([Quantity]) AS Quantity FROM [StockPurchased] LEFT OUTER JOIN Product ON (Product.Uid = [StockPurchased].ProductID) WHERE Product.SubscriptionID = @SubscriptionID GROUP BY [StockPurchased].[ProductID], [StockPurchased].[Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], ((-1) * SUM([Quantity])) AS Quantity FROM [StockSold] LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) WHERE Product.SubscriptionID = @SubscriptionID GROUP BY [StockSold].[ProductID], [StockSold].[Account]) StockQuantity GROUP BY [ProductID], [Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], 0 AS Quantity, ((-1) * (([StockSold].[Quantity] * [StockSold].[UnitPrice]) + ([StockSold].[Quantity]*[StockSold].[SalesTax]) + ([StockSold].[Quantity]*[StockSold].[IncomeTax]))) AS StockValue FROM StockSold LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND [StockSold].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'SalesTransaction') UNION SELECT [ProductID],[Account],0 AS [Quantity], (([Quantity] * [UnitPrice]) + ([Quantity] * [SalesTax]) + ([Quantity] * [IncomeTax])) AS StockValue FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account],SUM([Quantity]) AS [Quantity], AVG([StockPurchased].[UnitPrice]) AS [UnitPrice], AVG([StockPurchased].[SalesTax]) AS [SalesTax], AVG([StockPurchased].[IncomeTax]) AS [IncomeTax] FROM StockPurchased LEFT OUTER JOIN Product ON (Product.Uid = StockPurchased.ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND [StockPurchased].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'StockTransaction') GROUP BY [ProductID], [StockPurchased].[Account]) PurchasedStock) CorrectedStock GROUP BY [ProductID], [Account]) AvailableStock) CalculatedStock LEFT OUTER JOIN Product ON (Product.Uid = [ProductID]) WHERE [Product].Name LIKE @ProductName) AS Stock WHERE RNUMBER BETWEEN (((@PAGENUMBER - 1) * @PAGESIZE)+1) AND (@PAGENUMBER * @PAGESIZE) ORDER BY ProductName {0}", orderBy);
                countQuery = string.Format("SELECT COUNT(0) FROM (SELECT [ProductID],[Account], Quantity, StockValue, StockValue / (IIF(Quantity = 0,1,Quantity)) AS CalculatedPrice FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, SUM(StockValue) StockValue FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, StockValue = ISNULL((SELECT SUM(DrAmount) - SUM(CrAmount) AS StockValue FROM [Transaction] WHERE (StockQuantity.Account = AccountNo AND SubscriptionID = @SubscriptionID)),0) FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account], SUM([Quantity]) AS Quantity FROM [StockPurchased] LEFT OUTER JOIN Product ON (Product.Uid = [StockPurchased].ProductID) WHERE Product.SubscriptionID = @SubscriptionID GROUP BY [StockPurchased].[ProductID], [StockPurchased].[Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], ((-1) * SUM([Quantity])) AS Quantity FROM [StockSold] LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) WHERE Product.SubscriptionID = @SubscriptionID GROUP BY [StockSold].[ProductID], [StockSold].[Account]) StockQuantity GROUP BY [ProductID], [Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], 0 AS Quantity, ((-1) * (([StockSold].[Quantity] * [StockSold].[UnitPrice]) + ([StockSold].[Quantity]*[StockSold].[SalesTax]) + ([StockSold].[Quantity]*[StockSold].[IncomeTax]))) AS StockValue FROM StockSold LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND [StockSold].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'SalesTransaction') UNION SELECT [ProductID],[Account],0 AS [Quantity], (([Quantity] * [UnitPrice]) + ([Quantity] * [SalesTax]) + ([Quantity] * [IncomeTax])) AS StockValue FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account],SUM([Quantity]) AS [Quantity], AVG([StockPurchased].[UnitPrice]) AS [UnitPrice], AVG([StockPurchased].[SalesTax]) AS [SalesTax], AVG([StockPurchased].[IncomeTax]) AS [IncomeTax] FROM StockPurchased LEFT OUTER JOIN Product ON (Product.Uid = StockPurchased.ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND [StockPurchased].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'StockTransaction') GROUP BY [ProductID], [StockPurchased].[Account]) PurchasedStock) CorrectedStock GROUP BY [ProductID], [Account]) AvailableStock) CalculatedStock LEFT OUTER JOIN Product ON (Product.Uid = [ProductID]) WHERE [Product].Name LIKE @ProductName;");

                SqlConnection sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand countCommand = new SqlCommand(countQuery, sqlConnection);

                countCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));
                countCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
                selectCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));
                selectCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));

                selectCommand.Parameters.Add(new SqlParameter("@PAGENUMBER", searchQuery.page));
                selectCommand.Parameters.Add(new SqlParameter("@PAGESIZE", searchQuery.size));

                selectCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;
                countCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    totalRecords = Util.ConvertToInt32(countCommand.ExecuteScalar(), 0);

                    if (totalRecords > 0)
                    {
                        dReader = selectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dReader != null)
                        {
                            while (dReader.Read())
                            {
                                contents.Add(ExtractContentFromReader(dReader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dReader != null)
                {
                    try
                    {
                        dReader.Close();
                    }
                    catch { }
                }
            }
            return contents;
        }
        #endregion

        #region GetStockList
        /// <summary>
        /// Gets stock list.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<Stock> GetStockList(SearchInput searchQuery)
        {
            List<Stock> contents = new List<Stock>();
            SqlDataReader dReader = null;            
            try
            {
                string topByClause = string.Empty;
                if (searchQuery.total > 0)
                {
                    topByClause = string.Format("TOP {0}", searchQuery.total);
                }

                string orderBy = (searchQuery.descend == null) ? "" : (((bool)searchQuery.descend) ? "DESC" : "");
                
                string selectQuery = string.Empty;
                string countQuery = string.Empty;
                string stockOperator = " = ";

                if (((List<SearchField>)searchQuery.options) != null)
                {
                    var stockOption = ((List<SearchField>)searchQuery.options).Where(l => l.field == "inOutStock").SingleOrDefault();
                    if (stockOption != null)
                    {
                        if (stockOption.value.Equals("inStock"))
                        {
                            stockOperator = " <> ";
                        }
                        else if (stockOption.value.Equals("outOfStock"))
                        {
                            stockOperator = " = ";
                        }
                    }
                }

                selectQuery = string.Format("SELECT {0} [ProductID], Quantity,[Product].Name AS ProductName,SalesPrice,[Product].Account,CalculatedPrice FROM (SELECT [ProductID], [Account], Quantity, StockValue, StockValue / (IIF(Quantity = 0,1,Quantity)) AS CalculatedPrice FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, SUM(StockValue) StockValue FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, StockValue = ISNULL((SELECT SUM(DrAmount) - SUM(CrAmount) AS StockValue FROM [Transaction] WHERE (StockQuantity.Account = AccountNo AND SubscriptionID = @SubscriptionID)),0) FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account], SUM([Quantity]) AS Quantity FROM [StockPurchased] LEFT OUTER JOIN Product ON (Product.Uid = [StockPurchased].ProductID) GROUP BY [StockPurchased].[ProductID], [StockPurchased].[Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], ((-1) * SUM([Quantity])) AS Quantity FROM [StockSold] LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) GROUP BY [StockSold].[ProductID], [StockSold].[Account]) StockQuantity GROUP BY [ProductID], [Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], 0 AS Quantity, ((-1) * (([StockSold].[Quantity] * [StockSold].[UnitPrice]) + ([StockSold].[Quantity]*[StockSold].[SalesTax]) + ([StockSold].[Quantity]*[StockSold].[IncomeTax]))) AS StockValue FROM StockSold WHERE [StockSold].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'SalesTransaction') UNION SELECT [ProductID],[Account],0 AS [Quantity], (([Quantity] * [UnitPrice]) + ([Quantity] * [SalesTax]) + ([Quantity] * [IncomeTax])) AS StockValue FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account],SUM([Quantity]) AS [Quantity], AVG([StockPurchased].[UnitPrice]) AS [UnitPrice], AVG([StockPurchased].[SalesTax]) AS [SalesTax], AVG([StockPurchased].[IncomeTax]) AS [IncomeTax] FROM StockPurchased WHERE [StockPurchased].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'StockTransaction') GROUP BY [ProductID], [Account]) PurchasedStock) CorrectedStock GROUP BY [ProductID], [Account]) AvailableStock) CalculatedStock LEFT OUTER JOIN Product ON (Product.Uid = [ProductID]) WHERE [Product].Name LIKE @ProductName AND Published = 1 AND Product.SubscriptionID = @SubscriptionID AND Quantity {1} 0", topByClause, stockOperator);
                
                SqlConnection sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);
                selectCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));
                selectCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));

                selectCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dReader = selectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dReader != null)
                    {
                        while (dReader.Read())
                        {
                            contents.Add(ExtractContentFromReader(dReader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dReader != null)
                {
                    try
                    {
                        dReader.Close();
                    }
                    catch { }
                }
            }

            return contents;
        }
        #endregion

        #region GetStockTotal
        /// <summary>
        /// Gets stock total.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public int GetStockTotal(SearchInput searchQuery)
        {
            SqlConnection sqlConnection = null;
            int totalRecords = 0;
            try
            {   
                string countQuery = string.Empty;
                string stockOperator = " = ";

                if (((List<SearchField>)searchQuery.options) != null)
                {
                    var stockOption = ((List<SearchField>)searchQuery.options).Where(l => l.field == "inOutStock").SingleOrDefault();
                    if (stockOption != null)
                    {
                        if (stockOption.value.Equals("inStock"))
                        {
                            stockOperator = " <> ";
                        }
                        else if (stockOption.value.Equals("outOfStock"))
                        {
                            stockOperator = " = ";
                        }
                    }

                    countQuery = string.Format("SELECT COUNT(0) FROM (SELECT [ProductID], AvailableStock.[Account], Quantity, StockValue, StockValue / (IIF(Quantity = 0,1,Quantity)) AS UnitPrice FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, SUM(StockValue) StockValue FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, StockValue = ISNULL((SELECT SUM(DrAmount) - SUM(CrAmount) AS StockValue FROM [Transaction] WHERE (StockQuantity.Account = AccountNo AND SubscriptionID = @SubscriptionID)),0) FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account], SUM([Quantity]) AS Quantity FROM [StockPurchased] LEFT OUTER JOIN Product ON (Product.Uid = [StockPurchased].ProductID) GROUP BY [StockPurchased].[ProductID], [StockPurchased].[Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], ((-1) * SUM([Quantity])) AS Quantity FROM [StockSold] LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) GROUP BY [StockSold].[ProductID], [StockSold].[Account]) StockQuantity GROUP BY [ProductID], [Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], 0 AS Quantity, ((-1) * (([StockSold].[Quantity] * [StockSold].[UnitPrice]) + ([StockSold].[Quantity]*[StockSold].[SalesTax]) + ([StockSold].[Quantity]*[StockSold].[IncomeTax]))) AS StockValue FROM StockSold WHERE [StockSold].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'SalesTransaction') UNION SELECT [ProductID],[Account],0 AS [Quantity], (([Quantity] * [UnitPrice]) + ([Quantity] * [SalesTax]) + ([Quantity] * [IncomeTax])) AS StockValue FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account],SUM([Quantity]) AS [Quantity], AVG([StockPurchased].[UnitPrice]) AS [UnitPrice], AVG([StockPurchased].[SalesTax]) AS [SalesTax], AVG([StockPurchased].[IncomeTax]) AS [IncomeTax] FROM StockPurchased WHERE [StockPurchased].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'StockTransaction') GROUP BY [ProductID], [Account]) PurchasedStock) CorrectedStock GROUP BY [ProductID], [Account]) AvailableStock LEFT OUTER JOIN [Product] ON ([Product].Uid = AvailableStock.ProductID) WHERE Quantity {0} 0 AND [Product].Published = 1 AND Product.SubscriptionID = @SubscriptionID) CalculatedStock", stockOperator);
                }
                else
                {
                    countQuery = string.Format("SELECT COUNT(0) FROM (SELECT [ProductID], AvailableStock.[Account], Quantity, StockValue, StockValue / (IIF(Quantity = 0,1,Quantity)) AS UnitPrice FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, SUM(StockValue) StockValue FROM (SELECT [ProductID], [Account], SUM(Quantity) Quantity, StockValue = ISNULL((SELECT SUM(DrAmount) - SUM(CrAmount) AS StockValue FROM [Transaction] WHERE (StockQuantity.Account = AccountNo AND SubscriptionID = @SubscriptionID)),0) FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account], SUM([Quantity]) AS Quantity FROM [StockPurchased] LEFT OUTER JOIN Product ON (Product.Uid = [StockPurchased].ProductID) GROUP BY [StockPurchased].[ProductID], [StockPurchased].[Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], ((-1) * SUM([Quantity])) AS Quantity FROM [StockSold] LEFT OUTER JOIN Product ON (Product.Uid = [StockSold].ProductID) GROUP BY [StockSold].[ProductID], [StockSold].[Account]) StockQuantity GROUP BY [ProductID], [Account] UNION SELECT [StockSold].[ProductID], [StockSold].[Account], 0 AS Quantity, ((-1) * (([StockSold].[Quantity] * [StockSold].[UnitPrice]) + ([StockSold].[Quantity]*[StockSold].[SalesTax]) + ([StockSold].[Quantity]*[StockSold].[IncomeTax]))) AS StockValue FROM StockSold WHERE [StockSold].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'SalesTransaction') UNION SELECT [ProductID],[Account],0 AS [Quantity], (([Quantity] * [UnitPrice]) + ([Quantity] * [SalesTax]) + ([Quantity] * [IncomeTax])) AS StockValue FROM (SELECT [StockPurchased].[ProductID], [StockPurchased].[Account],SUM([Quantity]) AS [Quantity], AVG([StockPurchased].[UnitPrice]) AS [UnitPrice], AVG([StockPurchased].[SalesTax]) AS [SalesTax], AVG([StockPurchased].[IncomeTax]) AS [IncomeTax] FROM StockPurchased WHERE [StockPurchased].[StockID] NOT IN (SELECT [Transaction].[ReferenceID] FROM [Transaction] WHERE Reference = 'StockTransaction') GROUP BY [ProductID], [Account]) PurchasedStock) CorrectedStock GROUP BY [ProductID], [Account]) AvailableStock LEFT OUTER JOIN [Product] ON ([Product].Uid = AvailableStock.ProductID) WHERE [Product].Published = 1 AND Product.SubscriptionID = @SubscriptionID) CalculatedStock");
                }

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand countCommand = new SqlCommand(countQuery, sqlConnection);
                
                countCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));

                countCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    totalRecords = Util.ConvertToInt32(countCommand.ExecuteScalar(), 0);
                }
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
                    catch { }
                }
            }
            return totalRecords;
        }
        #endregion

        #region GetStockSummary
        /// <summary>
        /// Gets stock summary.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<Stock> GetStockSummary(SearchInput searchQuery)
        {
            List<Stock> contents = new List<Stock>();
            SqlDataReader dReader = null;

            try
            {                        
                string selectQuery = "SELECT ProductID,ProductName,Account,SalesPrice,SUM(Quantity) Quantity,SUM(Cost) TotalCost,SUM(Cost) / SUM(Quantity) CalculatedPrice FROM (SELECT ProductID,Product.Name AS ProductName,Product.Account,Product.SalesPrice,Quantity,(Quantity * StockPurchased.UnitPrice) + (Quantity * StockPurchased.Expense) + (Quantity * SalesTax) + (Quantity * IncomeTax) Cost FROM StockPurchased LEFT OUTER JOIN Product ON (Product.Uid = ProductID) WHERE CAST(StockTime AS DateTime) >= @StartTime AND CAST(StockTime AS DateTime) < @EndTime AND Product.SubscriptionID = @SubscriptionID AND Product.Name LIKE @ProductName) Stock GROUP BY ProductID,ProductName,Account,SalesPrice ORDER BY ProductName";

                SqlConnection sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);

                selectCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));
                selectCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));

                selectCommand.Parameters.Add(new SqlParameter("@StartTime", searchQuery.starttime));
                selectCommand.Parameters.Add(new SqlParameter("@EndTime", searchQuery.endtime));

                selectCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dReader = selectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dReader != null)
                    {
                        while (dReader.Read())
                        {
                            contents.Add(ExtractContentFromReader(dReader, false));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dReader != null)
                {
                    try
                    {
                        dReader.Close();
                    }
                    catch { }
                }
            }

            return new List<Stock>(contents);
        }
        #endregion

        #region GetStockDaily
        /// <summary>
        /// Gets daily stock report.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<Stock> GetStockDaily(SearchInput searchQuery)
        {
            List<Stock> contents = new List<Stock>();
            SqlDataReader dReader = null;
            
            try
            {
                string orderBy = (searchQuery.descend == null) ? "" : (((bool)searchQuery.descend) ? "DESC" : "");
                
                string selectQuery = string.Format("SELECT StockTime,ProductID,Product.Name AS ProductName,Product.Account, Product.SalesPrice,SUM(Quantity) Quantity,SUM(Cost) TotalCost,SUM(Cost) / SUM(Quantity) CalculatedPrice FROM (SELECT CONCAT(DATEPART(YYYY,StockTime),'-',DATEPART(MM,StockTime),'-',DATEPART(DD,StockTime)) StockTime,ProductID,Quantity,(Quantity * UnitPrice) + (Quantity * Expense) + (Quantity * SalesTax) + (Quantity * IncomeTax) Cost FROM StockPurchased) Stock LEFT OUTER JOIN Product ON (Product.Uid = ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND Product.Name LIKE @ProductName AND CAST(StockTime AS DateTime) >= @StartTime AND CAST(StockTime AS DateTime) < @EndTime GROUP BY StockTime,ProductID,Product.Name,Product.Account,Product.SalesPrice ORDER BY CAST(StockTime AS DateTime),Name", orderBy);

                SqlConnection sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);

                selectCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));
                selectCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));

                selectCommand.Parameters.Add(new SqlParameter("@StartTime", searchQuery.starttime));
                selectCommand.Parameters.Add(new SqlParameter("@EndTime", searchQuery.endtime));

                selectCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;
                
                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dReader = selectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dReader != null)
                    {
                        while (dReader.Read())
                        {
                            contents.Add(ExtractContentFromReader(dReader, true));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dReader != null)
                {
                    try
                    {
                        dReader.Close();
                    }
                    catch { }
                }
            }

            return new List<Stock>(contents);
        }
        #endregion

        #region GetStockMonthly
        /// <summary>
        /// Gets monthly stock report.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<Stock> GetStockMonthly(SearchInput searchQuery)
        {
            List<Stock> contents = new List<Stock>();
            SqlDataReader dReader = null;

            searchQuery.starttime = new DateTime(searchQuery.starttime.Value.Year, searchQuery.starttime.Value.Month, searchQuery.starttime.Value.Day, 0, 0, 0);

            try
            {
                string orderBy = (searchQuery.descend == null) ? "" : (((bool)searchQuery.descend) ? "DESC" : "");

                string selectQuery = string.Format("SELECT StockTime,ProductID,Product.Name AS ProductName,Product.Account,Product.SalesPrice,SUM(Quantity) Quantity,SUM(Cost) TotalCost,SUM(Cost) / SUM(Quantity) CalculatedPrice FROM (SELECT CONCAT(DATEPART(YYYY,StockTime),'-',DATEPART(MM,StockTime)) StockTime,ProductID,Quantity,(Quantity * UnitPrice) + (Quantity * Expense) + (Quantity * SalesTax) + (Quantity * IncomeTax) Cost FROM StockPurchased) Stock LEFT OUTER JOIN Product ON (Product.Uid = ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND CAST(CONCAT(StockTime,'-01') AS DateTime) >= @StartTime AND CAST(CONCAT(StockTime,'-01') AS DateTime) < @EndTime AND Product.Name LIKE @ProductName GROUP BY StockTime,ProductID,Product.Name,Product.Account,Product.SalesPrice ORDER BY CAST(CONCAT(StockTime,'-01') AS DateTime)", orderBy);

                SqlConnection sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);

                selectCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));
                selectCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));
                selectCommand.Parameters.Add(new SqlParameter("@StartTime", searchQuery.starttime));
                selectCommand.Parameters.Add(new SqlParameter("@EndTime", searchQuery.endtime));

                //selectCommand.Parameters.Add(new SqlParameter("@Month", searchQuery.starttime));

                //selectCommand.Parameters.Add(new SqlParameter("@PAGENUMBER", searchQuery.page));
                //selectCommand.Parameters.Add(new SqlParameter("@PAGESIZE", searchQuery.size));

                selectCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dReader = selectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dReader != null)
                    {
                        while (dReader.Read())
                        {
                            contents.Add(ExtractContentFromReader(dReader,true));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dReader != null)
                {
                    try
                    {
                        dReader.Close();
                    }
                    catch { }
                }
            }

            return new List<Stock>(contents);
        }
        #endregion

        #region GetStockByPlace
        /// <summary>
        /// Gets stock by place.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<Stock> GetStockByPlace(SearchInput searchQuery)
        {
            List<Stock> contents = new List<Stock>();
            SqlDataReader dReader = null;

            try
            {
                string orderBy = (searchQuery.descend == null) ? "" : (((bool)searchQuery.descend) ? "DESC" : "");

                string selectQuery = string.Format("SELECT StockTime,PlaceID,Place.Name AS PlaceName,ProductID,Product.Name AS ProductName,Product.Account, Product.SalesPrice,SUM(Quantity) Quantity,SUM(Cost) TotalCost,SUM(Cost) / SUM(Quantity) UnitPrice FROM (SELECT CONCAT(DATEPART(YYYY,StockTime),'-',DATEPART(MM,StockTime),'-',DATEPART(DD,StockTime)) StockTime,ProductID,PlaceID,Quantity,(Quantity * UnitPrice) + (Quantity * Expense) + (Quantity * SalesTax) + (Quantity * IncomeTax) Cost FROM StockPurchased WHERE Quantity > 0) Stock LEFT OUTER JOIN Product ON (Product.Uid = ProductID) LEFT OUTER JOIN Place ON (Place.Uid = PlaceID) WHERE Product.SubscriptionID = @SubscriptionID AND (Product.Name LIKE @ProductName OR Place.Name LIKE @PlaceName) AND CAST(StockTime AS DateTime) >= @StartTime AND CAST(StockTime AS DateTime) < @EndTime GROUP BY StockTime,PlaceID,Place.Name,ProductID,PlaceID,Product.Name,Product.Account,Product.SalesPrice ORDER BY CAST(StockTime AS DateTime),Place.Name,Product.Name", orderBy);

                SqlConnection sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);

                selectCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));
                selectCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));
                selectCommand.Parameters.Add(new SqlParameter("@PlaceName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));

                selectCommand.Parameters.Add(new SqlParameter("@StartTime", searchQuery.starttime));
                selectCommand.Parameters.Add(new SqlParameter("@EndTime", searchQuery.endtime));

                //selectCommand.Parameters.Add(new SqlParameter("@PAGENUMBER", searchQuery.page));
                //selectCommand.Parameters.Add(new SqlParameter("@PAGESIZE", searchQuery.size));

                selectCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dReader = selectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dReader != null)
                    {
                        while (dReader.Read())
                        {
                            contents.Add(ExtractPlaceContentFromReader(dReader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dReader != null)
                {
                    try
                    {
                        dReader.Close();
                    }
                    catch { }
                }
            }

            return new List<Stock>(contents);
        }
        #endregion

        #region GetSalesSummary
        /// <summary>
        /// Gets sales summary.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<Stock> GetSalesSummary(SearchInput searchQuery)
        {
            List<Stock> contents = new List<Stock>();
            SqlDataReader dReader = null;

            try
            {
                string selectQuery = "SELECT ProductID,ProductName,Account,SalesPrice,SUM(Quantity) Quantity,SUM(Cost) TotalCost,SUM(Cost) / SUM(Quantity) CalculatedPrice FROM (SELECT ProductID,Product.Name AS ProductName,Product.Account, Product.SalesPrice,Quantity,(Quantity * StockSold.UnitPrice) + (Quantity * StockSold.Expense) + (Quantity * SalesTax) + (Quantity * IncomeTax) Cost FROM StockSold LEFT OUTER JOIN Product ON (Product.Uid = ProductID) WHERE CAST(StockTime AS DateTime) >= @StartTime AND CAST(StockTime AS DateTime) < @EndTime AND Product.SubscriptionID = @SubscriptionID AND Product.Name LIKE @ProductName) Stock GROUP BY ProductID,ProductName,Account,SalesPrice ORDER BY ProductName";

                SqlConnection sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);

                selectCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));
                selectCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));

                selectCommand.Parameters.Add(new SqlParameter("@StartTime", searchQuery.starttime));
                selectCommand.Parameters.Add(new SqlParameter("@EndTime", searchQuery.endtime));

                selectCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dReader = selectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dReader != null)
                    {
                        while (dReader.Read())
                        {
                            contents.Add(ExtractContentFromReader(dReader, false));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dReader != null)
                {
                    try
                    {
                        dReader.Close();
                    }
                    catch { }
                }
            }

            return new List<Stock>(contents);
        }
        #endregion

        #region GetSalesDaily
        /// <summary>
        /// Gets daily sales report.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<Stock> GetSalesDaily(SearchInput searchQuery)
        {
            List<Stock> contents = new List<Stock>();
            SqlDataReader dReader = null;

            try
            {
                string orderBy = (searchQuery.descend == null) ? "" : (((bool)searchQuery.descend) ? "DESC" : "");

                string selectQuery = string.Format("SELECT StockTime,ProductID,Product.Name AS ProductName,Product.Account, Product.SalesPrice AS SalesPrice,SUM(Quantity) Quantity,SUM(Cost) TotalCost,SUM(Cost) / SUM(Quantity) CalculatedPrice FROM (SELECT CONCAT(DATEPART(YYYY,StockTime),'-',DATEPART(MM,StockTime),'-',DATEPART(DD,StockTime)) StockTime,ProductID,Quantity,(Quantity * UnitPrice) + (Quantity * Expense) + (Quantity * SalesTax) + (Quantity * IncomeTax) Cost FROM StockSold) Stock LEFT OUTER JOIN Product ON (Product.Uid = ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND Product.Name LIKE @ProductName AND CAST(StockTime AS DateTime) >= @StartTime AND CAST(StockTime AS DateTime) < @EndTime GROUP BY StockTime,ProductID,Product.Name,Product.Account,Product.SalesPrice ORDER BY CAST(StockTime AS DateTime),Name", orderBy);

                SqlConnection sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);

                selectCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));
                selectCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));

                selectCommand.Parameters.Add(new SqlParameter("@StartTime", searchQuery.starttime));
                selectCommand.Parameters.Add(new SqlParameter("@EndTime", searchQuery.endtime));

                selectCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dReader = selectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dReader != null)
                    {
                        while (dReader.Read())
                        {
                            contents.Add(ExtractContentFromReader(dReader, true));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dReader != null)
                {
                    try
                    {
                        dReader.Close();
                    }
                    catch { }
                }
            }

            return new List<Stock>(contents);
        }
        #endregion

        #region GetSalesMonthly
        /// <summary>
        /// Gets sales monthly report.
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public List<Stock> GetSalesMonthly(SearchInput searchQuery)
        {
            List<Stock> contents = new List<Stock>();
            SqlDataReader dReader = null;

            searchQuery.starttime = new DateTime(searchQuery.starttime.Value.Year, searchQuery.starttime.Value.Month, searchQuery.starttime.Value.Day, 0, 0, 0);

            try
            {
                string orderBy = (searchQuery.descend == null) ? "" : (((bool)searchQuery.descend) ? "DESC" : "");

                string selectQuery = string.Format("SELECT StockTime,ProductID,Product.Name AS ProductName,Product.Account,Product.SalesPrice,SUM(Quantity) Quantity,SUM(Cost) TotalCost,SUM(Cost) / SUM(Quantity) CalculatedPrice FROM (SELECT CONCAT(DATEPART(YYYY,StockTime),'-',DATEPART(MM,StockTime)) StockTime,ProductID,Quantity,(Quantity * UnitPrice) + (Quantity * Expense) + (Quantity * SalesTax) + (Quantity * IncomeTax) Cost FROM StockSold) Stock LEFT OUTER JOIN Product ON (Product.Uid = ProductID) WHERE Product.SubscriptionID = @SubscriptionID AND CAST(CONCAT(StockTime,'-01') AS DateTime) >= @StartTime AND CAST(CONCAT(StockTime,'-01') AS DateTime) < @EndTime AND Product.Name LIKE @ProductName GROUP BY StockTime,ProductID,Product.Name,Product.Account,Product.SalesPrice ORDER BY CAST(CONCAT(StockTime,'-01') AS DateTime)", orderBy);

                SqlConnection sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);

                SqlCommand selectCommand = new SqlCommand(selectQuery, sqlConnection);

                selectCommand.Parameters.Add(new SqlParameter("@SubscriptionID", searchQuery.key));
                selectCommand.Parameters.Add(new SqlParameter("@ProductName", searchQuery.keyword == null ? "%" : ("%" + searchQuery.keyword + "%")));
                selectCommand.Parameters.Add(new SqlParameter("@StartTime", searchQuery.starttime));
                selectCommand.Parameters.Add(new SqlParameter("@EndTime", searchQuery.endtime));

                //selectCommand.Parameters.Add(new SqlParameter("@PAGENUMBER", searchQuery.page));
                //selectCommand.Parameters.Add(new SqlParameter("@PAGESIZE", searchQuery.size));

                selectCommand.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dReader = selectCommand.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dReader != null)
                    {
                        while (dReader.Read())
                        {
                            contents.Add(ExtractContentFromReader(dReader, true));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dReader != null)
                {
                    try
                    {
                        dReader.Close();
                    }
                    catch { }
                }
            }

            return new List<Stock>(contents);
        }
        #endregion
    }
}