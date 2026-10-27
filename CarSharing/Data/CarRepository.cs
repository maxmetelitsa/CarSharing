using System;
using System.Collections.Generic;
using System.Data.SQLite;
using CarSharing.CarsInfo;
using CarSharing.Enums;
using CarSharing.SQL_Manager;

namespace CarSharing.Data
{
    public interface ICarRepository
    {
        IReadOnlyList<Car> GetAll();

        /// <summary>The car with the given ID, or null if there is none.</summary>
        Car GetById(int id);
    }

    public class CarRepository : ICarRepository
    {
        private const string SelectCars =
            "SELECT ID, Name, FuelType, TransmissionType, CarType, Brand, Colour, YearOfManufacture, Number, ImageID, HourPrice FROM Cars";

        public IReadOnlyList<Car> GetAll()
        {
            return Query(SelectCars + " ORDER BY ID", null);
        }

        public Car GetById(int id)
        {
            IReadOnlyList<Car> cars = Query(SelectCars + " WHERE ID = @ID", id);
            return cars.Count > 0 ? cars[0] : null;
        }

        private static IReadOnlyList<Car> Query(string query, int? id)
        {
            var cars = new List<Car>();

            using (SQLiteConnection connection = SQLiteManager.CreateOpenConnection())
            using (var command = new SQLiteCommand(query, connection))
            {
                if (id.HasValue)
                {
                    command.Parameters.AddWithValue("@ID", id.Value);
                }

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cars.Add(new Car(
                            Convert.ToInt32(reader["ID"]),
                            reader["Name"].ToString(),
                            (Fuel)Convert.ToInt32(reader["FuelType"]),
                            (Transmission)Convert.ToInt32(reader["TransmissionType"]),
                            (CarType)Convert.ToInt32(reader["CarType"]),
                            (Brand)Convert.ToInt32(reader["Brand"]),
                            reader["Colour"].ToString(),
                            Convert.ToInt32(reader["YearOfManufacture"]),
                            reader["Number"].ToString(),
                            Convert.ToInt32(reader["ImageID"]),
                            // The column is called HourPrice for historical reasons; it stores the price per day.
                            Convert.ToDouble(reader["HourPrice"])));
                    }
                }
            }

            return cars;
        }
    }
}
