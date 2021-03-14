using DataAccessNET5.Models;
using DataAccessNET5.Listing;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Inventory
{
    /// <summary>
    /// Product repository.
    /// </summary>
    public class ProductRepository : EntityRepository<Product>
    {
        public ProductRepository(string connectionString)
            : base(new ApplicationContext(connectionString))
        {

        }

        public ProductRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {
            
        }

        protected override Product GenerateNewKey(Product contentObject)
        {
            contentObject.Uid = Guid.NewGuid();
            return contentObject;
        }

        protected override object GetTypedKey(object key)
        {
            try
            {
                return Guid.Parse((string)key);
            }
            catch { }

            return key;
        }

        public override Product Read(object id)
        {
            try
            {
                return base.Read(Guid.Parse(id.ToString()));
            }
            catch
            { }

            return entitySet.Where(l => (l.Code == id.ToString())).SingleOrDefault(); ;
        }

        protected override IQueryable<Product> QueryRecords(IQueryable<Product> query, SearchInput searchQuery = null)
        {
            Expression<Func<Product, bool>> condition = null;
            if (searchQuery != null)
            {
                if (!string.IsNullOrEmpty(searchQuery.key))
                {
                    condition = l => (l.Code == searchQuery.key);
                    query = query.Where(condition);
                }

                if (searchQuery.options != null)
                {
                    var topProduct = ((List<SearchField>)searchQuery.options).Where(l => l.field == "top").SingleOrDefault();
                    if (topProduct != null)
                    {
                        query = query.Where(l => l.Top == true);
                    }

                    var favouriteProduct = ((List<SearchField>)searchQuery.options).Where(l => l.field == "favourite").SingleOrDefault();
                    if (favouriteProduct != null)
                    {
                        query = query.Where(l => l.Favourite == true);
                    }

                    var discountProduct = ((List<SearchField>)searchQuery.options).Where(l => l.field == "discount").SingleOrDefault();
                    if (discountProduct != null)
                    {
                        query = query.Where(l => l.Discount > 0);
                    }
                }

                searchQuery.keyword = (string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword);
                condition = l => (l.Code.Contains(searchQuery.keyword) || l.Name.Contains(searchQuery.keyword) || l.Description.Contains(searchQuery.keyword) || l.Category.Contains(searchQuery.keyword) || l.Color.Contains(searchQuery.keyword));
                query = query.Where(condition);
            }

            return query;
        }

        protected override IOrderedQueryable<Product> SortRecords(IQueryable<Product> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<Product> orderInterface = null;
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

        #region ListProducts
        /// <summary>
        /// Gets list of products by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<ProductListItem> ListProducts(string name)
        {
            List<ProductListItem> contents = null;
            IQueryable<Product> iQueryable = null;

            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    iQueryable = ((ApplicationContext)context).Products.Where(l => l.Name == name);
                }
                else
                {
                    iQueryable = ((ApplicationContext)context).Products;
                }

                contents = iQueryable.ToList<Product>()
                    .Select(l =>
                    {
                        return new ProductListItem
                        {
                            Uid = l.Uid,
                            Name = l.Name,
                            SalesPrice = l.SalesPrice
                        };
                    }).ToList<ProductListItem>();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return contents;
        } 
        #endregion

        #region ExtractContentFromReader
        public Product ExtractContentFromReader(SqlDataReader dataReader, bool product = false)
        {
            try
            {
                if (dataReader != null)
                {
                    var content = new Product
                    {
                        Uid = Guid.Parse(dataReader["Uid"].ToString()),
                        UserId = Convert.ToString(dataReader["UserId"]),
                        Category = Convert.ToString(dataReader["Category"]),
                        Manufacturer = Convert.ToString(dataReader["Manufacturer"]),
                        Code = Convert.ToString(dataReader["Code"]),
                        Color = Convert.ToString(dataReader["Color"]),
                        Description = Convert.ToString(dataReader["Description"]),
                        Name = Convert.ToString(dataReader["Name"]),
                        Picture = (dataReader["Picture"] == DBNull.Value ? null : ((byte[])dataReader["Picture"])),
                        StockLevel = dataReader["StockLevel"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["StockLevel"]),
                        SalesPrice = dataReader["SalesPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["SalesPrice"]),
                        Commission = dataReader["Commission"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["Commission"]),
                        Discount = dataReader["Discount"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["Discount"]),
                        IncomeTax = dataReader["IncomeTax"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["IncomeTax"]),
                        SalesTax = dataReader["SalesTax"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["SalesTax"]),
                        AvailableQuantity = dataReader["AvailableQuantity"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["AvailableQuantity"]),
                        RowVersion = (dataReader["RowVersion"] == DBNull.Value) ? null : ((byte[])dataReader["RowVersion"])
                    };

                    if (!product)
                    {
                        content.ProductName = dataReader["ProductName"] == DBNull.Value ? "" : Convert.ToString(dataReader["ProductName"]);
                        content.PlaceName = dataReader["PlaceName"] == DBNull.Value ? "" : Convert.ToString(dataReader["PlaceName"]);
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
        #endregion

        #region GetProduct
        public Product GetProduct(SearchInput searchInput)
        {
            SqlConnection sqlConnection = null;
            SqlDataReader dataReader = null;
            Product product = null;

            try
            {
                string query = string.Format("SELECT [Uid],ProductID,[UserId],[Category],[Manufacturer],[Code],[Color],[Description],[Name],[Picture],[StockLevel],[SalesPrice],[Commission],[Discount],[IncomeTax],[SalesTax],[RowVersion],AvailableQuantity FROM (SELECT ProductID, SUM(PurchasedQuantity) - SUM(SoldQuantity) AS AvailableQuantity FROM (SELECT [StockPurchased].[Uid], StockPurchased.ProductID, ISNULL([Quantity],0) AS PurchasedQuantity, 0 AS SoldQuantity FROM StockPurchased LEFT OUTER JOIN Product ON (StockPurchased.ProductID = Product.Uid) LEFT OUTER JOIN StockTransaction ON (StockTransaction.[Uid] = StockPurchased.StockID) WHERE (ProductName LIKE @ProductName OR Product.Code LIKE @ProductName) AND PlaceName LIKE @PlaceName AND StockTransaction.Active = 1 UNION SELECT [StockSold].[Uid], [StockSold].ProductID, 0 AS PurchasedQuantity, ISNULL([Quantity],0) AS SoldQuantity FROM [StockSold] LEFT OUTER JOIN Product ON ([Product].[Uid] = [StockSold].[ProductID]) LEFT OUTER JOIN SalesTransaction ON (SalesTransaction.[Uid] = StockSold.StockID) WHERE ([ProductName] LIKE @ProductName OR [Product].Code LIKE @ProductName) AND PlaceName LIKE @PlaceName AND SalesTransaction.Active = 1) AvailableStock GROUP BY [ProductID]) Stock LEFT OUTER JOIN Product ON (Product.Uid = ProductID);");

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.Add(new SqlParameter("@ProductName", searchInput.key));
                command.Parameters.Add(new SqlParameter("@PlaceName", searchInput.source));

                command.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dataReader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dataReader != null)
                    {
                        if (dataReader.Read())
                        {
                            product = ExtractContentFromReader(dataReader, true);
                        }
                    }
                }

                return product;
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

        #region GetStockCount
        public int GetStockCount(SearchInput searchInput)
        {
            SqlConnection sqlConnection = null;
            int stockCount = 0;

            searchInput.keyword = string.IsNullOrEmpty(searchInput.keyword) ? "%" : string.Format("%{0}%", searchInput.keyword);
            searchInput.source = string.IsNullOrEmpty(searchInput.source) ? "%" : string.Format("%{0}%", searchInput.source);

            string stockPredicate = "Having ISNULL((SUM(PurchasedQuantity) - SUM(SoldQuantity)),0) > 0";
            if (searchInput.enabled == null)
            {
                stockPredicate = "";
            }
            else
            {
                if ((bool)searchInput.enabled)
                {
                    stockPredicate = "Having ISNULL((SUM(PurchasedQuantity) - SUM(SoldQuantity)),0) <= 0";
                }
            }

            try
            {
                string query = string.Format("SELECT COUNT (0) FROM (SELECT ProductName, PlaceName, ISNULL((SUM(PurchasedQuantity) - SUM(SoldQuantity)),0) AS AvailableQuantity FROM (SELECT [StockPurchased].[Uid], ProductName, PlaceName, ISNULL([Quantity],0) AS PurchasedQuantity, 0 AS SoldQuantity FROM [StockPurchased] INNER JOIN [Product] ON ([StockPurchased].[ProductID] = [Product].[Uid]) INNER JOIN [StockTransaction] ON ([StockPurchased].StockID = [StockTransaction].Uid) WHERE ([Product].Code LIKE @ProductName OR [ProductName] LIKE @ProductName) AND [PlaceName] LIKE @PlaceName AND [StockTransaction].Active = 1 UNION SELECT [StockSold].[Uid], ProductName, PlaceName, 0 AS PurchasedQuantity, ISNULL([Quantity],0) AS SoldQuantity FROM [StockSold] INNER JOIN [Product] ON ([StockSold].[ProductID] = [Product].[Uid]) INNER JOIN [SalesTransaction] ON ([StockSold].StockID = [SalesTransaction].Uid)  WHERE ([Product].Code LIKE @ProductName OR [ProductName] LIKE @ProductName) AND [PlaceName] LIKE @PlaceName AND [SalesTransaction].Active = 1) AvailableStock GROUP BY ProductName, PlaceName {0}) StockCount;", stockPredicate);

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.Add(new SqlParameter("@ProductName", searchInput.keyword));
                command.Parameters.Add(new SqlParameter("@PlaceName", searchInput.source));

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

        #region GetProducts
        public List<Product> GetProducts(SearchInput searchInput)
        {
            SqlConnection sqlConnection = null;
            SqlDataReader dataReader = null;
            List<Product> products = new List<Product>();

            searchInput.keyword = string.IsNullOrEmpty(searchInput.keyword) ? "%" : string.Format("%{0}%", searchInput.keyword);
            searchInput.source = string.IsNullOrEmpty(searchInput.source) ? "%" : string.Format("%{0}%", searchInput.source);

            string stockPredicate = "Having ISNULL((SUM(PurchasedQuantity) - SUM(SoldQuantity)),0) > 0";
            if (searchInput.enabled == null)
            {
                stockPredicate = "";
            }
            else
            {
                if ((bool)searchInput.enabled)
                {
                    stockPredicate = "Having ISNULL((SUM(PurchasedQuantity) - SUM(SoldQuantity)),0) <= 0";
                }
            }

            try
            {
                string query = string.Format("SELECT RNUMBER, ProductName, PlaceName, [Uid],[UserId],[Category],[Manufacturer],[Code],[Color],[Description],[Name],[Picture],[StockLevel],[SalesPrice],[Commission],[Discount],[IncomeTax],[SalesTax],[RowVersion],AvailableQuantity FROM (SELECT ROW_NUMBER() OVER (ORDER BY ProductName ASC) AS RNUMBER,ProductName, PlaceName, ISNULL((SUM(PurchasedQuantity) - SUM(SoldQuantity)),0) AS AvailableQuantity FROM (SELECT [StockPurchased].[Uid], ProductName, PlaceName, ISNULL([Quantity],0) AS PurchasedQuantity, 0 AS SoldQuantity FROM [StockPurchased] INNER JOIN [Product] ON ([StockPurchased].[ProductID] = [Product].[Uid]) INNER JOIN [StockTransaction] ON ([StockTransaction].Uid = [StockPurchased].StockID) WHERE ([Product].Code LIKE @ProductName OR [ProductName] LIKE @ProductName) AND [PlaceName] LIKE @PlaceName AND [StockTransaction].Active = 1 UNION SELECT [StockSold].[Uid], ProductName, PlaceName, 0 AS PurchasedQuantity, ISNULL([Quantity],0) AS SoldQuantity FROM [StockSold] INNER JOIN [Product] ON ([StockSold].[ProductID] = [Product].[Uid]) INNER JOIN [SalesTransaction] ON ([SalesTransaction].Uid = [StockSold].StockID) WHERE ([Product].Code LIKE @ProductName OR [ProductName] LIKE @ProductName) AND [PlaceName] LIKE @PlaceName AND [SalesTransaction].Active = 1) AvailableStock GROUP BY ProductName, PlaceName {0}) StockList LEFT OUTER JOIN Product ON (Product.Name = ProductName) WHERE RNUMBER BETWEEN (((@PAGENUMBER - 1) * @PAGESIZE)+1) AND (@PAGENUMBER * @PAGESIZE) ORDER BY AvailableQuantity DESC;", stockPredicate);

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.Add(new SqlParameter("@ProductName", searchInput.keyword));
                command.Parameters.Add(new SqlParameter("@PlaceName", searchInput.source));
                command.Parameters.Add(new SqlParameter("@PAGENUMBER", searchInput.page));
                command.Parameters.Add(new SqlParameter("@PAGESIZE", searchInput.size));

                command.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dataReader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            products.Add(ExtractContentFromReader(dataReader));
                        }
                    }
                }

                return products;
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

        #region ListStockProducts
        public List<Product> ListStockProducts(SearchInput searchInput)
        {
            SqlConnection sqlConnection = null;
            SqlDataReader dataReader = null;
            List<Product> products = new List<Product>();

            searchInput.keyword = string.IsNullOrEmpty(searchInput.keyword) ? "%" : string.Format("%{0}%", searchInput.keyword);
            searchInput.source = string.IsNullOrEmpty(searchInput.source) ? "%" : string.Format("%{0}%", searchInput.source);

            try
            {
                string query = string.Format("SELECT TOP 50 ProductID, ProductName, PlaceName, [Uid],[UserId],[Category],[Manufacturer],[Code],[Color],[Description],[Name],[Picture],[StockLevel],[SalesPrice],[Commission],[Discount],[IncomeTax],[SalesTax],[RowVersion],AvailableQuantity FROM (SELECT ProductID, ProductName, PlaceName, ISNULL((SUM(PurchasedQuantity) - SUM(SoldQuantity)),0) AS AvailableQuantity FROM (SELECT [StockPurchased].[Uid], ProductID, ProductName, PlaceName, ISNULL([Quantity],0) AS PurchasedQuantity, 0 AS SoldQuantity FROM [StockPurchased] INNER JOIN [Product] ON ([StockPurchased].[ProductID] = [Product].[Uid]) INNER JOIN [StockTransaction] ON ([StockTransaction].[Uid] = [StockPurchased].StockID) WHERE ([Product].Code LIKE @ProductName OR [ProductName] LIKE @ProductName) AND [PlaceName] LIKE @PlaceName AND [StockTransaction].Active = 1 UNION SELECT [StockSold].[Uid], ProductID, ProductName, PlaceName, 0 AS PurchasedQuantity, ISNULL([Quantity],0) AS SoldQuantity FROM [StockSold] INNER JOIN [Product] ON ([StockSold].[ProductID] = [Product].[Uid]) INNER JOIN [SalesTransaction] ON ([SalesTransaction].[Uid] = [StockSold].StockID) WHERE ([Product].Code LIKE @ProductName OR [ProductName] LIKE @ProductName) AND [PlaceName] LIKE @PlaceName AND [SalesTransaction].Active = 1) AvailableStock GROUP BY ProductID, ProductName, PlaceName) StockList LEFT OUTER JOIN Product ON (Product.Uid = ProductID) ORDER BY AvailableQuantity DESC;");

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.Add(new SqlParameter("@ProductName", searchInput.keyword));
                command.Parameters.Add(new SqlParameter("@PlaceName", searchInput.source));

                command.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dataReader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            products.Add(ExtractContentFromReader(dataReader));
                        }
                    }
                }

                return products;
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

        #region ExtractContentWithoutPictureFromReader
        public Product ExtractContentWithoutPictureFromReader(SqlDataReader dataReader, bool product = false)
        {
            try
            {
                if (dataReader != null)
                {
                    var content = new Product
                    {
                        Uid = Guid.Parse(dataReader["Uid"].ToString()),
                        UserId = Convert.ToString(dataReader["UserId"]),
                        Category = Convert.ToString(dataReader["Category"]),
                        Manufacturer = Convert.ToString(dataReader["Manufacturer"]),
                        Code = Convert.ToString(dataReader["Code"]),
                        Color = Convert.ToString(dataReader["Color"]),
                        Description = Convert.ToString(dataReader["Description"]),
                        Name = Convert.ToString(dataReader["Name"]),
                        StockLevel = dataReader["StockLevel"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["StockLevel"]),
                        SalesPrice = dataReader["SalesPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["SalesPrice"]),
                        PublishedPrice = dataReader["PublishedPrice"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["PublishedPrice"]),
                        Commission = dataReader["Commission"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["Commission"]),
                        Favourite = dataReader["Favourite"] == DBNull.Value ? false : Convert.ToBoolean(dataReader["Favourite"]),
                        Top = dataReader["Top"] == DBNull.Value ? false : Convert.ToBoolean(dataReader["Top"]),
                        Discount = dataReader["Discount"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["Discount"]),
                        IncomeTax = dataReader["IncomeTax"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["IncomeTax"]),
                        SalesTax = dataReader["SalesTax"] == DBNull.Value ? 0 : Convert.ToDecimal(dataReader["SalesTax"]),
                        AvailableQuantity = dataReader["AvailableQuantity"] == DBNull.Value ? 0 : Convert.ToInt32(dataReader["AvailableQuantity"]),
                        RowVersion = (dataReader["RowVersion"] == DBNull.Value) ? null : ((byte[])dataReader["RowVersion"])
                    };

                    if (!product)
                    {
                        content.ProductName = dataReader["ProductName"] == DBNull.Value ? "" : Convert.ToString(dataReader["ProductName"]);
                        content.PlaceName = dataReader["PlaceName"] == DBNull.Value ? "" : Convert.ToString(dataReader["PlaceName"]);
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
        #endregion

        #region GetProductsWithQuantityCount
        public int GetProductsWithQuantityTotal(SearchInput searchInput)
        {
            SqlConnection sqlConnection = null;
            int stockCount = 0;

            searchInput.key = string.IsNullOrEmpty(searchInput.key) ? "%" : string.Format("%{0}%", searchInput.key);
            searchInput.keyword = string.IsNullOrEmpty(searchInput.keyword) ? "%" : string.Format("%{0}%", searchInput.keyword);
            searchInput.source = string.IsNullOrEmpty(searchInput.source) ? "%" : string.Format("%{0}%", searchInput.source);

            string top = "%";
            string favourite = "%";
            string discountQueryPart = string.Empty;

            if (searchInput.options != null)
            {
                var topProduct = ((List<SearchField>)searchInput.options).Where(l => l.field == "top").SingleOrDefault();
                if (topProduct != null)
                {
                    top = "1";
                }

                var favouriteProduct = ((List<SearchField>)searchInput.options).Where(l => l.field == "favourite").SingleOrDefault();
                if (favouriteProduct != null)
                {
                    favourite = "1";
                }

                var discountProduct = ((List<SearchField>)searchInput.options).Where(l => l.field == "discount").SingleOrDefault();
                if (discountProduct != null)
                {
                    discountQueryPart = "AND Discount > 0";
                }
            }

            try
            {
                string query = string.Format("SELECT COUNT(0) FROM (SELECT ROW_NUMBER() OVER (ORDER BY ProductName ASC) AS RNUMBER,ProductName, PlaceName, ISNULL((SUM(PurchasedQuantity) - SUM(SoldQuantity)),0) AS AvailableQuantity FROM (SELECT [StockPurchased].[Uid], ProductName, PlaceName, ISNULL([Quantity],0) AS PurchasedQuantity, 0 AS SoldQuantity FROM [StockPurchased] INNER JOIN [Product] ON ([StockPurchased].[ProductID] = [Product].[Uid]) INNER JOIN [StockTransaction] ON ([StockTransaction].Uid = [StockPurchased].StockID) WHERE ([Product].Code LIKE @ProductName OR [ProductName] LIKE @ProductName) AND [PlaceName] LIKE @PlaceName AND [StockTransaction].Active = 1 UNION SELECT [StockSold].[Uid], ProductName, PlaceName, 0 AS PurchasedQuantity, ISNULL([Quantity],0) AS SoldQuantity FROM [StockSold] INNER JOIN [Product] ON ([StockSold].[ProductID] = [Product].[Uid]) INNER JOIN [SalesTransaction] ON ([SalesTransaction].Uid = [StockSold].StockID) WHERE ([Product].Code LIKE @ProductName OR [ProductName] LIKE @ProductName) AND [PlaceName] LIKE @PlaceName AND [SalesTransaction].Active = 1) AvailableStock GROUP BY ProductName, PlaceName) StockList LEFT OUTER JOIN Product ON (Product.Name = ProductName) WHERE Category LIKE @Category AND CAST([Top] AS NVARCHAR(1)) LIKE @Top AND CAST([Favourite] AS NVARCHAR(1)) LIKE @Favourite {0};", discountQueryPart);

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.Add(new SqlParameter("@Category", searchInput.key));
                command.Parameters.Add(new SqlParameter("@Top", top));
                command.Parameters.Add(new SqlParameter("@Favourite", favourite));

                command.Parameters.Add(new SqlParameter("@ProductName", searchInput.keyword));
                command.Parameters.Add(new SqlParameter("@PlaceName", searchInput.source));

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

        #region GetProductsWithQuantity
        public List<Product> GetProductsWithQuantity(SearchInput searchInput)
        {
            SqlConnection sqlConnection = null;
            SqlDataReader dataReader = null;
            List<Product> products = new List<Product>();

            searchInput.key = string.IsNullOrEmpty(searchInput.key) ? "%" : string.Format("%{0}%", searchInput.key);
            searchInput.keyword = string.IsNullOrEmpty(searchInput.keyword) ? "%" : string.Format("%{0}%", searchInput.keyword);
            searchInput.source = string.IsNullOrEmpty(searchInput.source) ? "%" : string.Format("%{0}%", searchInput.source);

            string top = "%";
            string favourite = "%";
            string discountQueryPart = string.Empty;

            if (searchInput.options != null)
            {
                var topProduct = ((List<SearchField>)searchInput.options).Where(l => l.field == "top").SingleOrDefault();
                if (topProduct != null)
                {
                    top = "1";
                }

                var favouriteProduct = ((List<SearchField>)searchInput.options).Where(l => l.field == "favourite").SingleOrDefault();
                if (favouriteProduct != null)
                {
                    favourite = "1";
                }

                var discountProduct = ((List<SearchField>)searchInput.options).Where(l => l.field == "discount").SingleOrDefault();
                if (discountProduct != null)
                {
                    discountQueryPart = "AND Discount > 0";
                }
            }

            try
            {
                string query = string.Format("SELECT RNUMBER, ProductName, PlaceName, [Uid],[UserId],[Category],[Manufacturer],[Code],[Color],[Description],[Name],[Picture],[StockLevel],[SalesPrice],[PublishedPrice],[Commission],[Discount],[IncomeTax],[SalesTax],[Favourite],[Top],[RowVersion],AvailableQuantity FROM (SELECT ROW_NUMBER() OVER (ORDER BY ProductName ASC) AS RNUMBER,ProductName, PlaceName, ISNULL((SUM(PurchasedQuantity) - SUM(SoldQuantity)),0) AS AvailableQuantity FROM (SELECT [StockPurchased].[Uid], ProductName, PlaceName, ISNULL([Quantity],0) AS PurchasedQuantity, 0 AS SoldQuantity FROM [StockPurchased] INNER JOIN [Product] ON ([StockPurchased].[ProductID] = [Product].[Uid]) INNER JOIN [StockTransaction] ON ([StockTransaction].Uid = [StockPurchased].StockID) WHERE ([Product].Code LIKE @ProductName OR [ProductName] LIKE @ProductName) AND [PlaceName] LIKE @PlaceName AND [StockTransaction].Active = 1 UNION SELECT [StockSold].[Uid], ProductName, PlaceName, 0 AS PurchasedQuantity, ISNULL([Quantity],0) AS SoldQuantity FROM [StockSold] INNER JOIN [Product] ON ([StockSold].[ProductID] = [Product].[Uid]) INNER JOIN [SalesTransaction] ON ([SalesTransaction].Uid = [StockSold].StockID) WHERE ([Product].Code LIKE @ProductName OR [ProductName] LIKE @ProductName) AND [PlaceName] LIKE @PlaceName AND [SalesTransaction].Active = 1) AvailableStock GROUP BY ProductName, PlaceName) StockList LEFT OUTER JOIN Product ON (Product.Name = ProductName) WHERE Category LIKE @Category AND CAST([Top] AS NVARCHAR(1)) LIKE @Top AND CAST([Favourite] AS NVARCHAR(1)) LIKE @Favourite {0} AND RNUMBER BETWEEN (((@PAGENUMBER - 1) * @PAGESIZE)+1) AND (@PAGENUMBER * @PAGESIZE) ORDER BY AvailableQuantity DESC;", discountQueryPart);

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.Add(new SqlParameter("@Category", searchInput.key));
                command.Parameters.Add(new SqlParameter("@Top", top));
                command.Parameters.Add(new SqlParameter("@Favourite", favourite));

                command.Parameters.Add(new SqlParameter("@ProductName", searchInput.keyword));
                command.Parameters.Add(new SqlParameter("@PlaceName", searchInput.source));
                command.Parameters.Add(new SqlParameter("@PAGENUMBER", searchInput.page));
                command.Parameters.Add(new SqlParameter("@PAGESIZE", searchInput.size));

                command.CommandTimeout = ((ApplicationContext)context).Database.GetDbConnection().ConnectionTimeout;

                sqlConnection.Open();
                if (sqlConnection.State == System.Data.ConnectionState.Open)
                {
                    dataReader = command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (dataReader != null)
                    {
                        while (dataReader.Read())
                        {
                            products.Add(ExtractContentWithoutPictureFromReader(dataReader));
                        }
                    }
                }

                return products;
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
    }
}
