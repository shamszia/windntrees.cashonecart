using DataAccessNET5.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WindnTrees.CRUDS.Repository.Core;
using WindnTrees.ICRUDS.Standard;

namespace DataAccessNET5.Repositories.Order
{
    public class OrderRepository : EntityRepository<DataAccessNET5.Models.Order>
    {
        public OrderRepository(string connectionString)
            : base(new ApplicationContext(connectionString), "read:OrderItems")
        {

        }

        public OrderRepository(ApplicationContext dbContext, string relatedObjects = "")
            : base(dbContext, relatedObjects)
        {

        }

        protected override object GetTypedKey(object key)
        {
            return Guid.Parse((string)key);
        }

        protected override IQueryable<DataAccessNET5.Models.Order> QueryRecords(IQueryable<DataAccessNET5.Models.Order> query, SearchInput searchQuery = null)
        {
            Expression<Func<DataAccessNET5.Models.Order, bool>> condition = null;
            if (searchQuery.starttime >= new DateTime(1800, 1, 1) && searchQuery.endtime >= new DateTime(1800, 1, 1))
            {
                condition = l => (l.OrderTime >= searchQuery.starttime && l.OrderTime < searchQuery.endtime);
                query = query.Where(condition);
            }

            long orderNumber = 0;
            try
            {
                orderNumber = long.Parse(searchQuery.keyword);
                condition = l => l.OrderNo == orderNumber;
                query = query.Where(condition);
            }
            catch { }

            if (orderNumber == 0)
            {
                var options = (List<SearchField>)searchQuery.options;
                if (options != null)
                {
                    var orderTypeOption = options.Where(l => l.field == "OrderType").SingleOrDefault();
                    if (orderTypeOption != null)
                    {
                        query = query.Where(l => l.OrderType == orderTypeOption.value);
                    }

                    var orderStatusTypeOption = options.Where(l => l.field == "OrderStatus").SingleOrDefault();
                    if (orderStatusTypeOption != null)
                    {
                        query = query.Where(l => l.Status == orderStatusTypeOption.value);
                    }
                }

                searchQuery.keyword = string.IsNullOrEmpty(searchQuery.keyword) ? "" : searchQuery.keyword;

                condition = l => (l.Title.Contains(searchQuery.keyword) || 
                l.Company.Contains(searchQuery.keyword) || 
                l.Address.Contains(searchQuery.keyword) || 
                l.City.Contains(searchQuery.keyword) || 
                l.Country.Contains(searchQuery.keyword) || 
                l.Status.Contains(searchQuery.keyword));

                query = query.Where(condition);
            }
            return query;
        }

        protected override IOrderedQueryable<DataAccessNET5.Models.Order> SortRecords(IQueryable<DataAccessNET5.Models.Order> query, SearchInput searchQuery = null)
        {
            IOrderedQueryable<DataAccessNET5.Models.Order> orderInterface = null;
            if (searchQuery != null)
            {
                if (searchQuery.descend == null ? false : ((bool)searchQuery.descend))
                {
                    orderInterface = query.OrderByDescending(l => l.OrderTime);
                }
                else
                {
                    orderInterface = query.OrderBy(l => l.OrderTime);
                }
            }

            return orderInterface;
        }

        public override DataAccessNET5.Models.Order Read(object id)
        {
            try
            {
                Guid.Parse(id.ToString());
                return base.Read(id);
            }
            catch { }

            var longId = long.Parse(id.ToString());

            IQueryable<DataAccessNET5.Models.Order> queryInterface = entitySet;
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

            var result = queryInterface.Where(l => (l.OrderNo == longId)).SingleOrDefault();

            return result;
        }

        public override DataAccessNET5.Models.Order Create(DataAccessNET5.Models.Order contentObject)
        {
            try
            {
                DateTime orderTime = DateTime.UtcNow;

                contentObject.OrderTime = orderTime;
                contentObject.Uid = Guid.NewGuid();

                foreach (var orderItem in contentObject.OrderItems)
                {
                    orderItem.Uid = Guid.NewGuid();
                    orderItem.OrderId = contentObject.Uid;
                }

                DataAccessNET5.Models.Order entityObject = entitySet.Add(contentObject).Entity;

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

        public override DataAccessNET5.Models.Order Update(DataAccessNET5.Models.Order contentObject)
        {
            try
            {
                var exitingOrder = ((DbSet<DataAccessNET5.Models.Order>)entitySet).Include("OrderItems").Where(l => l.Uid == contentObject.Uid).SingleOrDefault();
                if (exitingOrder != null)
                {
                    exitingOrder.OrderItems.Clear();
                    context.SaveChanges();

                    foreach (var orderItem in contentObject.OrderItems)
                    {
                        orderItem.Order = null;

                        orderItem.Uid = Guid.NewGuid();
                        orderItem.OrderId = contentObject.Uid;
                        exitingOrder.OrderItems.Add(orderItem);
                    }

                    context.Entry(exitingOrder).CurrentValues.SetValues(contentObject);

                    if (context.SaveChanges() > 0)
                    {
                        contentObject = PostCreate(contentObject);
                        return contentObject;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DataAccessNET5.Models.Order> ListOrders(SearchInput searchInput)
        {
            return List(searchInput);
        }

        public List<DataAccessNET5.Models.Order> ListOrdersPlaced(SearchInput searchInput)
        {
            searchInput.options = new List<SearchField> {
                new SearchField{ field = "OrderStatus", value = "Placed" }
            };

            return List(searchInput);
        }

        #region CopyOrder
        public DataAccessNET5.Models.Order CopyOrder(DataAccessNET5.Models.Order contentObject)
        {
            try
            {
                DateTime orderTime = DateTime.UtcNow;

                contentObject.OrderTime = orderTime;
                contentObject.Uid = Guid.NewGuid();
                contentObject.Status = "Placed";

                foreach (var orderItem in contentObject.OrderItems)
                {
                    orderItem.Uid = Guid.NewGuid();
                    orderItem.OrderId = contentObject.Uid;
                }

                DataAccessNET5.Models.Order entityObject = entitySet.Add(contentObject).Entity;

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
        #endregion

        #region CancelOrder
        public int CancelOrder(string orderID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string query = string.Format("UPDATE [Order] SET Status = 'Canceled' WHERE Uid = @Uid");

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand command = new SqlCommand(query, sqlConnection);

                command.Parameters.Add(new SqlParameter("@Uid", orderID));

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

        #region DeleteOrder
        public int DeleteOrder(string orderID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string deleteQuery = string.Format("DELETE FROM [Order] WHERE Uid = @Uid");

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection);

                deleteCommand.Parameters.Add(new SqlParameter("@Uid", orderID));

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

        #region DeleteOrderItems
        public int DeleteOrderItems(string orderID)
        {
            SqlConnection sqlConnection = null;
            try
            {
                string deleteQuery = string.Format("DELETE FROM [OrderItem] WHERE OrderID = @OrderID");

                sqlConnection = new SqlConnection(((ApplicationContext)context).Database.GetDbConnection().ConnectionString);
                SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection);

                deleteCommand.Parameters.Add(new SqlParameter("@OrderID", orderID));

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
    }
}
