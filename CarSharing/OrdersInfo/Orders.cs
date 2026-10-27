using CarSharing.CarsInfo;
using CarSharing.SQL_Manager;
using CarSharing.UsersInfo;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace CarSharing.OrdersInfo
{
    public class Orders
    {
        public List<Order> orders;
        SQLiteManager manager;

        public Orders()
        {
            orders = new List<Order>();
            manager = new SQLiteManager();
        }
        public void AddOrder(int userID, int carID, DateTime startDate, DateTime endDate, double totalPrice)
        {
            manager.OpenConnection();

            string insertQuery = "INSERT INTO Orders (UserID, CarID, StartDate, EndDate, TotalPrice) VALUES (@UserID, @CarID, @StartDate, @EndDate, @TotalPrice)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@CarID", carID);
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                command.Parameters.AddWithValue("@TotalPrice", totalPrice);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    orders.Add(new Order(userID, carID, startDate, endDate, totalPrice));
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

            foreach (Order order in orders)
            {
                string updateQuery = $"UPDATE Orders SET UserID = @UserID, CarID = @CarID, StartDate = @StartDate, EndDate = @EndDate, TotalPrice = @TotalPrice WHERE ID = @ID";

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, manager.Connection))
                {
                    command.Parameters.AddWithValue("@UserID", order.UserID);
                    command.Parameters.AddWithValue("@CarID", order.CarID);
                    command.Parameters.AddWithValue("@StartDate", order.StartDate);
                    command.Parameters.AddWithValue("@EndDate", order.EndDate);
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

            string selectQuery = "SELECT * FROM Orders";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Int64 userID = reader.GetInt64(reader.GetOrdinal("UserID"));
                        Int64 carID = reader.GetInt64(reader.GetOrdinal("CarID"));
                        DateTime startDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                        DateTime endDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                        double totalPrice = reader.GetDouble(reader.GetOrdinal("TotalPrice"));

                        orders.Add(new Order((int)userID, (int)carID, startDate, endDate, totalPrice));
                    }
                }
            }

            manager.CloseConnection();
        }

    }
}