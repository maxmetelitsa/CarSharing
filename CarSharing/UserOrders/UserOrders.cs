using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using CarSharing.OrdersInfo;
using CarSharing.SQL_Manager;

namespace CarSharing.UserOrders
{
    public class UserOrders
    {
        public List<UserOrder> userOrders;
        public List<UserOrder> currentUserOrders;
        SQLiteManager manager;
        public UserOrders()
        {
            userOrders = new List<UserOrder>();
            currentUserOrders = new List<UserOrder>();

            manager = new SQLiteManager();
        }
        public void AddUserOrder(int orderID, int userID, int rentalContractNumber)
        {
            manager.OpenConnection();

            string insertQuery = "INSERT INTO UserOrders (OrderID, UserID, RentalСontractNumber) VALUES (@OrderID, @UserID, @RentalСontractNumber)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@OrderID", orderID);
                command.Parameters.AddWithValue("@UserID", userID);
                command.Parameters.AddWithValue("@RentalСontractNumber", rentalContractNumber);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    userOrders.Add(new UserOrder(orderID, userID, rentalContractNumber));
                    manager.CloseConnection();
                }
                else
                {
                    manager.CloseConnection();

                }
            }
        }

        public void GetUserOrders(int id)
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM UserOrders WHERE UserID = @UserID";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@UserID", id);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        Int64 orderID = reader.GetInt64(reader.GetOrdinal("OrderID"));
                        Int64 rentalID = reader.GetInt64(reader.GetOrdinal("RentalСontractNumber"));

                        currentUserOrders.Add(new UserOrder((int)orderID, id, (int)rentalID));
                    }
                }
            }

            manager.CloseConnection();
        }
    }
}