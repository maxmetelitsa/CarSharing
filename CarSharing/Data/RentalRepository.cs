using System;
using System.Data.SQLite;
using CarSharing.SQL_Manager;

namespace CarSharing.Data
{
    /// <summary>Everything that has to be stored when a customer books a car.</summary>
    public class RentalRecord
    {
        public int CustomerId { get; set; }
        public int CarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double TotalPrice { get; set; }

        // Documents from the form are saved to the customer's profile for next time.
        public string PassportNumber { get; set; }
        public string IdentificationNumber { get; set; }
        public string LicenseSeries { get; set; }
        public string LicenseNumber { get; set; }
    }

    public interface IRentalRepository
    {
        /// <summary>True if the car has no booking overlapping the given period.</summary>
        bool IsAvailable(int carId, DateTime startDate, DateTime endDate);

        /// <summary>Saves the booking in one transaction and returns the new order (contract) number.</summary>
        int SaveRental(RentalRecord rental);
    }

    public class RentalRepository : IRentalRepository
    {
        private const string DateFormat = "yyyy-MM-dd";

        public bool IsAvailable(int carId, DateTime startDate, DateTime endDate)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM [Order]
                WHERE CarID = @CarID
                  AND StartDate <= @EndDate
                  AND EndDate >= @StartDate";

            using (SQLiteConnection connection = SQLiteManager.CreateOpenConnection())
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CarID", carId);
                command.Parameters.AddWithValue("@StartDate", startDate.ToString(DateFormat));
                command.Parameters.AddWithValue("@EndDate", endDate.ToString(DateFormat));

                return Convert.ToInt64(command.ExecuteScalar()) == 0;
            }
        }

        public int SaveRental(RentalRecord rental)
        {
            if (rental == null)
            {
                throw new ArgumentNullException(nameof(rental));
            }

            using (SQLiteConnection connection = SQLiteManager.CreateOpenConnection())
            using (SQLiteTransaction transaction = connection.BeginTransaction())
            {
                Execute(connection, transaction,
                    "INSERT INTO [Order] (UserID, CarID, StartDate, EndDate, TotalPrice) " +
                    "VALUES (@UserID, @CarID, @StartDate, @EndDate, @TotalPrice)",
                    ("@UserID", rental.CustomerId),
                    ("@CarID", rental.CarId),
                    ("@StartDate", rental.StartDate.ToString(DateFormat)),
                    ("@EndDate", rental.EndDate.ToString(DateFormat)),
                    ("@TotalPrice", rental.TotalPrice));

                int orderId;
                using (var command = new SQLiteCommand("SELECT last_insert_rowid()", connection, transaction))
                {
                    orderId = Convert.ToInt32(command.ExecuteScalar());
                }

                Execute(connection, transaction,
                    "INSERT INTO UserOrders (OrderID, UserID, RentalСontractNumber) VALUES (@OrderID, @UserID, @OrderID)",
                    ("@OrderID", orderId), ("@UserID", rental.CustomerId));

                Execute(connection, transaction,
                    "UPDATE Users SET PassportNumber = @PassportNumber, IdentificationNumber = @IdentificationNumber, " +
                    "LicenseSeries = @LicenseSeries, LicenseNumber = @LicenseNumber WHERE ID = @UserID",
                    ("@PassportNumber", rental.PassportNumber),
                    ("@IdentificationNumber", rental.IdentificationNumber),
                    ("@LicenseSeries", rental.LicenseSeries),
                    ("@LicenseNumber", rental.LicenseNumber),
                    ("@UserID", rental.CustomerId));

                transaction.Commit();
                return orderId;
            }
        }

        private static void Execute(SQLiteConnection connection, SQLiteTransaction transaction, string sql,
            params (string Name, object Value)[] parameters)
        {
            using (var command = new SQLiteCommand(sql, connection, transaction))
            {
                foreach (var (name, value) in parameters)
                {
                    command.Parameters.AddWithValue(name, value);
                }

                command.ExecuteNonQuery();
            }
        }
    }
}
