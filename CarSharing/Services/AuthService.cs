using System;
using System.Data.SQLite;
using CarSharing.Enums;
using CarSharing.Security;
using CarSharing.SQL_Manager;
using CarSharing.UsersInfo;

namespace CarSharing.Services
{
    public interface IAuthService
    {
        /// <summary>Checks the credentials and, on success, sets CurrentUserManager.CurrentUser.</summary>
        LoginResult SignIn(string email, string password);
    }

    /// <summary>Signs in customers (Users table) and administrators (Administrators table).</summary>
    public class AuthService : IAuthService
    {
        public LoginResult SignIn(string email, string password)
        {
            using (SQLiteConnection connection = SQLiteManager.CreateOpenConnection())
            {
                CurrentUser customer = FindCustomer(connection, email, password);
                if (customer != null)
                {
                    CurrentUserManager.CurrentUser = customer;
                    return LoginResult.Customer;
                }

                CurrentUser administrator = FindAdministrator(connection, email, password);
                if (administrator != null)
                {
                    CurrentUserManager.CurrentUser = administrator;
                    return LoginResult.Administrator;
                }
            }

            return LoginResult.Failed;
        }

        private static CurrentUser FindCustomer(SQLiteConnection connection, string email, string password)
        {
            const string query = "SELECT * FROM Users WHERE Email = @Email LIMIT 1";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    string passwordHash = reader["Password"].ToString();
                    if (!PasswordHasher.Verify(password, passwordHash))
                    {
                        return null;
                    }

                    return new CurrentUser(
                        Convert.ToInt32(reader["ID"]),
                        reader["Name"].ToString(),
                        reader["Surname"].ToString(),
                        reader["Email"].ToString(),
                        passwordHash,
                        reader["PassportNumber"].ToString(),
                        reader["IdentificationNumber"].ToString(),
                        reader["LicenseSeries"].ToString(),
                        reader["LicenseNumber"].ToString());
                }
            }
        }

        private static CurrentUser FindAdministrator(SQLiteConnection connection, string email, string password)
        {
            const string query = "SELECT * FROM Administrators WHERE Email = @Email LIMIT 1";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", email);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        return null;
                    }

                    string passwordHash = reader["PasswordHash"].ToString();
                    if (!PasswordHasher.Verify(password, passwordHash))
                    {
                        return null;
                    }

                    return new CurrentUser(reader["Name"].ToString(), reader["Surname"].ToString(), email, passwordHash);
                }
            }
        }
    }
}
