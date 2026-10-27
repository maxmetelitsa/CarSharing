using CarSharing.CarsInfo;
using CarSharing.Enums;
using CarSharing.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace CarSharing.OrdersInfo
{
    public class OrdersManager
    {
        public SQLiteManager manager;
        public Orders ordersList;

        public OrdersManager()
        {
            manager = new SQLiteManager();
            ordersList = new Orders();
        }

        public void AddOrder(int userID, int carID, DateTime startDate, DateTime endDate, double totalPrice)
        {
            manager.OpenConnection();

            string insertQuery = "INSERT INTO [Order] (UserID, CarID, StartDate, EndDate, TotalPrice) VALUES (@UserID, @CarID, @StartDate, @EndDate, @TotalPrice)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@CarID", carID);
                command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@TotalPrice", totalPrice);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    ordersList.orders.Add(new Order(userID, carID, startDate, endDate, totalPrice));
                    manager.CloseConnection();
                }
                else
                {
                    manager.CloseConnection();
                }
            }
        }

        public void UpdateAllOrders()
        {
            manager.OpenConnection();

            foreach (Order order in ordersList.orders)
            {
                string updateQuery = $"UPDATE [Order] SET UserID = @UserID, CarID = @CarID, StartDate = @StartDate, EndDate = @EndDate, TotalPrice = @TotalPrice WHERE ID = @ID";

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, manager.Connection))
                {
                    command.Parameters.AddWithValue("@UserID", order.UserID);
                    command.Parameters.AddWithValue("@CarID", order.CarID);
                    command.Parameters.AddWithValue("@StartDate", order.StartDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@EndDate", order.EndDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@TotalPrice", order.TotalPrice);
                    command.Parameters.AddWithValue("@ID", order.ID);

                    command.ExecuteNonQuery();
                }
            }

            manager.CloseConnection();
        }

        public void GetAllOrders()
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM [Order]";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Int64 userID = reader.GetInt64(reader.GetOrdinal("UserID"));
                        Int64 carID = reader.GetInt64(reader.GetOrdinal("CarID"));

                        string startDateText = reader.GetString(reader.GetOrdinal("StartDate"));
                        string endDateText = reader.GetString(reader.GetOrdinal("EndDate"));

                        DateTime startDate = DateTime.Parse(startDateText);
                        DateTime endDate = DateTime.Parse(endDateText);

                        double totalPrice = reader.GetDouble(reader.GetOrdinal("TotalPrice"));

                        ordersList.orders.Add(new Order((int)userID, (int)carID, startDate, endDate, totalPrice));
                    }
                }
            }

            manager.CloseConnection();
        }
        public void RemoveOrder(Order order, int ID)
        {
            manager.OpenConnection();

            string deleteQuery = "DELETE FROM [Order] WHERE ID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", ID);
                command.ExecuteNonQuery();
            }

            manager.CloseConnection();
            ordersList.orders.Remove(order);
        }
        public void ClearOrdersTable()
        {
            manager.OpenConnection();

            string deleteQuery = "DELETE FROM [Order]";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                command.ExecuteNonQuery();
            }

            manager.CloseConnection();

            ordersList.orders.Clear();
        }
    }
}
