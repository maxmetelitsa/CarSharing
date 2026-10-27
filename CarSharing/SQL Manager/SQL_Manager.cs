using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace CarSharing.SQL_Manager
{
    /// <summary>
    /// Owns the connection to the local SQLite database (CarHouse.db).
    /// </summary>
    public class SQLiteManager
    {
        private const string DatabaseFileName = "CarHouse.db";

        private static readonly Lazy<string> connectionString = new Lazy<string>(BuildConnectionString);

        private readonly SQLiteConnection connection;

        public SQLiteManager()
        {
            connection = new SQLiteConnection(connectionString.Value);
        }

        public SQLiteConnection Connection => connection;

        /// <summary>Opens a new connection; the caller disposes it (use with "using").</summary>
        public static SQLiteConnection CreateOpenConnection()
        {
            var newConnection = new SQLiteConnection(connectionString.Value);
            newConnection.Open();
            return newConnection;
        }

        public void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public void ExecuteNonQuery(string query)
        {
            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static string BuildConnectionString()
        {
            return $"Data Source={FindDatabasePath()};Version=3;";
        }

        /// <summary>
        /// Looks for the database next to the executable first, then in the project folder
        /// (bin\Debug\..\..), which is where it lives when running from Visual Studio.
        /// </summary>
        private static string FindDatabasePath()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Directory.GetParent(baseDirectory).Parent.Parent.FullName;

            string[] candidates =
            {
                Path.Combine(baseDirectory, DatabaseFileName),
                Path.Combine(projectDirectory, DatabaseFileName)
            };

            foreach (string path in candidates)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            throw new FileNotFoundException(
                $"Файл базы данных '{DatabaseFileName}' не найден. Проверенные пути: {string.Join("; ", candidates)}");
        }
    }
}
