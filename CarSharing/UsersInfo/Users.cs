using CarSharing.Security;
using CarSharing.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace CarSharing.UsersInfo
{
    public class Users
    {
        public List<User> users;
        public Users(SQLiteManager manager)
        {
            users = new List<User>();
            GetAllUsers(manager);
        }
        public bool AddUser(SQLiteManager manager, string name, string surname, string email, string password)
        {
            string passwordHash = PasswordHasher.Hash(password);

            manager.OpenConnection();

            string insertQuery = " INSERT INTO Users (Name, Surname, Email, Password, PassportNumber, IdentificationNumber, LicenseSeries, LicenseNumber) VALUES (@Name, @Surname, @Email, @Password, @PassportNumber, @IdentificationNumber, @LicenseSeries, @LicenseNumber);";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Surname", surname);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", passwordHash);
                command.Parameters.AddWithValue("@PassportNumber", null);
                command.Parameters.AddWithValue("@IdentificationNumber", null);
                command.Parameters.AddWithValue("@LicenseSeries", null);
                command.Parameters.AddWithValue("@LicenseNumber", null);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    users.Add(new User(name, surname, email, passwordHash));
                    manager.CloseConnection();
                    return true;
                }
                else
                {
                    manager.CloseConnection();
                    return false;
                }
            }
        }

        public void GetAllUsers(SQLiteManager manager)
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM Users";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader["Name"].ToString();
                        string surname = reader["Surname"].ToString();
                        string email = reader["Email"].ToString();
                        string password = reader["Password"].ToString();
                        users.Add(new User(name, surname, email, password));
                    }
                }
            }

            manager.CloseConnection();
        }

    }
}
