using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CarSharing.CarsInfo;
using CarSharing.Enums;
using CarSharing.SQL_Manager;
using CarSharing.UsersInfo;

namespace CarSharing.CarPark
{
    public class CarParkManager
    {
        public SQLiteManager manager;
        public Cars carsList;
        public Cars availableCarsList;

        public CarParkManager()
        {
            manager = new SQLiteManager();
            carsList = new Cars();
            availableCarsList = new Cars();
            GetAvailableCars();
        }
        public void GetAvailableCars()
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM Cars WHERE NOT EXISTS (SELECT 1 FROM [Order] WHERE Cars.ID = [Order].CarID)";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    //int lastId = 0;

                    while (reader.Read())
                    {
                        Int64 id = reader.GetInt64(reader.GetOrdinal("ID"));
                        string name = reader.GetString(reader.GetOrdinal("Name"));
                        Fuel fuelType = (Fuel)reader.GetInt32(reader.GetOrdinal("FuelType"));
                        Transmission transmission = (Transmission)reader.GetInt32(reader.GetOrdinal("TransmissionType"));
                        CarType carType = (CarType)reader.GetInt32(reader.GetOrdinal("CarType"));
                        Brand brand = (Brand)reader.GetInt32(reader.GetOrdinal("Brand"));
                        string colour = reader.GetString(reader.GetOrdinal("Colour"));
                        int yearOfManufacture = reader.GetInt32(reader.GetOrdinal("YearOfManufacture"));
                        string number = reader.GetString(reader.GetOrdinal("Number"));
                        int imageID = reader.GetInt32(reader.GetOrdinal("ImageID"));
                        double hourPrice = reader.GetDouble(reader.GetOrdinal("HourPrice"));

                        Car car = new Car((int)id, name, fuelType, transmission, carType, brand, colour, yearOfManufacture, number, imageID, hourPrice);

                        //if (lastId == 0)
                        //{
                        //    lastId = reader.GetInt32(reader.GetOrdinal("ID"));
                        //}
                        //else
                        //{
                        //    // Assign the same last ID to each car object in the list
                        //    car.ID = ++lastId;
                        //}

                        availableCarsList.cars.Add(car);
                    }
                }
            }

            manager.CloseConnection();

        }

        public void GetAllCars()
        {
            manager.OpenConnection();

            string selectQuery = "SELECT * FROM Cars";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name = reader.GetString(reader.GetOrdinal("Name"));
                        Fuel fuelType = (Fuel)reader.GetInt32(reader.GetOrdinal("FuelType"));
                        Transmission transmission = (Transmission)reader.GetInt32(reader.GetOrdinal("TransmissionType"));
                        CarType carType = (CarType)reader.GetInt32(reader.GetOrdinal("CarType"));
                        Brand brand = (Brand)reader.GetInt32(reader.GetOrdinal("Brand"));
                        string colour = reader.GetString(reader.GetOrdinal("Colour"));
                        int yearOfManufacture = reader.GetInt32(reader.GetOrdinal("YearOfManufacture"));
                        string number = reader.GetString(reader.GetOrdinal("Number"));
                        int imageID = reader.GetInt32(reader.GetOrdinal("ImageID"));
                        double hourPrice = reader.GetDouble(reader.GetOrdinal("HourPrice"));

                        carsList.cars.Add(new Car(name, fuelType, transmission, carType, brand, colour, yearOfManufacture, number, imageID, hourPrice));
                    }
                }
            }

            manager.CloseConnection();
        }

        public bool AddCar(string name, Fuel fuelType, Transmission transmission, CarType carType, Brand brand, string colour, int yearOfManufacture, string number, int imageID, double hourPrice)
        {
            GetAllCars();

            manager.OpenConnection();

            foreach (Car car in carsList.cars)
            {
                if (car.Number == number)
                {
                    manager.CloseConnection();
                    return false;
                }
            }

            string insertQuery = "INSERT INTO Cars (Name, FuelType, TransmissionType, CarType, Brand, Colour, YearOfManufacture, Number, ImageID, HourPrice) VALUES (@Name, @FuelType, @TransmissionType, @CarType, @Brand, @Colour, @YearOfManufacture, @Number, @ImageID, @HourPrice)";

            using (SQLiteCommand command = new SQLiteCommand(insertQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@FuelType", (int)fuelType);
                command.Parameters.AddWithValue("@TransmissionType", (int)transmission);
                command.Parameters.AddWithValue("@CarType", (int)carType);
                command.Parameters.AddWithValue("@Brand", (int)brand);
                command.Parameters.AddWithValue("@Colour", colour);
                command.Parameters.AddWithValue("@YearOfManufacture", yearOfManufacture);
                command.Parameters.AddWithValue("@Number", number);
                command.Parameters.AddWithValue("@ImageID", imageID);
                command.Parameters.AddWithValue("@HourPrice", hourPrice);

                command.ExecuteNonQuery();
            }

            // получаем CarID последнего автомобиля в Cars

            string selectQuery = "SELECT ID FROM Cars ORDER BY ID DESC LIMIT 1";
            int carId;

            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, manager.Connection))
            {
                using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        carId = reader.GetInt32(0);
                    }
                    else
                    {
                        // Обработка ситуации, если в таблице "cars" нет записей
                        // Например, выбрать значение по умолчанию или сгенерировать ошибку
                        carId = 0; // Пример значения по умолчанию
                    }
                }
            }

            string carClass = "";

            if (carType == CarType.Economy)
            {
                carClass = "EconomyClass";
            }
            else if (carType == CarType.Business)
            {
                carClass = "BusinessClass";
            }
            else if (carType == CarType.Coupe)
            {
                carClass = "CoupeClass";
            }
            else if (carType == CarType.Limousine)
            {
                carClass = "LimousineClass";
            }

            string insertQuery2 = $"INSERT INTO {carClass} (CarID, FuelType, TransmissionType, Brand, Colour, Number) VALUES (@CarID, @FuelType, @TransmissionType, @Brand, @Colour, @Number)";

            using (SQLiteCommand insertCommand = new SQLiteCommand(insertQuery2, manager.Connection))
            {
                insertCommand.Parameters.AddWithValue("@CarID", carId);
                insertCommand.Parameters.AddWithValue("@FuelType", (int)fuelType);
                insertCommand.Parameters.AddWithValue("@TransmissionType", (int)transmission);
                insertCommand.Parameters.AddWithValue("@Brand", (int)brand);
                insertCommand.Parameters.AddWithValue("@Colour", colour);
                insertCommand.Parameters.AddWithValue("@Number", number);

                insertCommand.ExecuteNonQuery();
            }

            manager.CloseConnection();

            carsList.cars.Add(new Car(name, fuelType, transmission, carType, brand, colour, yearOfManufacture, number, imageID, hourPrice));

            return true;
        }
        public void RemoveCar(Car car)
        {
            manager.OpenConnection();

            string selectQuery = "SELECT ID FROM Cars WHERE Name = @Name AND FuelType = @FuelType AND TransmissionType = @TransmissionType AND CarType = @CarType AND Brand = @Brand AND Colour = @Colour AND YearOfManufacture = @YearOfManufacture AND Number = @Number AND ImageID = @ImageID AND HourPrice = @HourPrice";

            int carId;

            using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, manager.Connection))
            {
                selectCommand.Parameters.AddWithValue("@Name", car.Name);
                selectCommand.Parameters.AddWithValue("@FuelType", (int)car.FuelType);
                selectCommand.Parameters.AddWithValue("@TransmissionType", (int)car.TransmissionType);
                selectCommand.Parameters.AddWithValue("@CarType", (int)car.CarType);
                selectCommand.Parameters.AddWithValue("@Brand", (int)car.Brand);
                selectCommand.Parameters.AddWithValue("@Colour", car.Colour);
                selectCommand.Parameters.AddWithValue("@YearOfManufacture", car.YearOfManufacture);
                selectCommand.Parameters.AddWithValue("@Number", car.Number);
                selectCommand.Parameters.AddWithValue("@ImageID", car.ImageID);
                selectCommand.Parameters.AddWithValue("@HourPrice", car.DayPrice);

                using (SQLiteDataReader reader = selectCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        carId = reader.GetInt32(0);
                    }
                    else
                    {
                        // Обработка ситуации, если в таблице "cars" нет записей
                        // Например, выбрать значение по умолчанию или сгенерировать ошибку
                        carId = 0; // Пример значения по умолчанию
                    }
                }
            }

            string deleteQuery = "DELETE FROM Cars WHERE Name = @Name AND FuelType = @FuelType AND TransmissionType = @TransmissionType AND CarType = @CarType AND Brand = @Brand AND Colour = @Colour AND YearOfManufacture = @YearOfManufacture AND Number = @Number AND ImageID = @ImageID AND HourPrice = @HourPrice";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@Name", car.Name);
                command.Parameters.AddWithValue("@FuelType", (int)car.FuelType);
                command.Parameters.AddWithValue("@TransmissionType", (int)car.TransmissionType);
                command.Parameters.AddWithValue("@CarType", (int)car.CarType);
                command.Parameters.AddWithValue("@Brand", (int)car.Brand);
                command.Parameters.AddWithValue("@Colour", car.Colour);
                command.Parameters.AddWithValue("@YearOfManufacture", car.YearOfManufacture);
                command.Parameters.AddWithValue("@Number", car.Number);
                command.Parameters.AddWithValue("@ImageID", car.ImageID);
                command.Parameters.AddWithValue("@HourPrice", car.DayPrice);

                command.ExecuteNonQuery();
            }

            string carClass = "";

            if (car.CarType == CarType.Economy)
            {
                carClass = "EconomyClass";
            }
            else if (car.CarType == CarType.Business)
            {
                carClass = "BusinessClass";
            }
            else if (car.CarType == CarType.Coupe)
            {
                carClass = "CoupeClass";
            }
            else if (car.CarType == CarType.Limousine)
            {
                carClass = "LimousineClass";
            }

            // carClass is one of four fixed table names chosen above, never user input.
            string deleteCarFromClass = $"DELETE FROM {carClass} WHERE CarID = @CarID";

            using (SQLiteCommand command = new SQLiteCommand(deleteCarFromClass, manager.Connection))
            {
                command.Parameters.AddWithValue("@CarID", carId);
                command.ExecuteNonQuery();
            }

            manager.CloseConnection();
            carsList.cars.Remove(car);
        }

        public bool ValidateDataCar(string name, Fuel fuelType, Transmission transmission, CarType carType, Brand brand, string colour, int yearOfManufacture, string number, int imageID, double hourPrice)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(Fuel), fuelType))
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(Transmission), transmission))
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(CarType), carType))
            {
                return false;
            }

            if (!Enum.IsDefined(typeof(Brand), brand))
            {
                return false;
            }

            if (string.IsNullOrEmpty(colour))
            {
                return false;
            }

            if (yearOfManufacture < 1900 || yearOfManufacture > DateTime.Now.Year)
            {
                return false;
            }

            if (string.IsNullOrEmpty(number))
            {
                return false;
            }

            if (imageID <= 0)
            {
                return false;
            }
            if (hourPrice <= 0)
            {
                return false;
            }

            return true;
        }

        public void ClearCarsTable()
        {
            manager.OpenConnection();

            string deleteQuery = "DELETE FROM Cars";

            using (SQLiteCommand command = new SQLiteCommand(deleteQuery, manager.Connection))
            {
                command.ExecuteNonQuery();
            }

            manager.CloseConnection();

            carsList.cars.Clear();
        }

    }
}
