using CarSharing.Functions;
using CarSharing.CarPark;
using CarSharing.CarsInfo;
using CarSharing.Enums;
using CarSharing.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Controls.Primitives;
using System.Security.AccessControl;
using CarSharing.Rent;
using CarSharing.Profile;
using CarSharing.About;
using System.Globalization;
using CarSharing.Authorization;

namespace CarSharing.Admin
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private CarParkManager carParkManager;
        private bool isEditMode = false;
        private string lastEditableCarID;
        public AdminWindow()
        {
            InitializeComponent();

            carParkManager = new CarParkManager();

            carParkManager.GetAllCars();

            ShowCarsInDataGrid(carsGrid, carParkManager);

            List<Fuel> fuelTypes = new List<Fuel> { Fuel.Petrol, Fuel.Diesel, Fuel.Gas, Fuel.Electricity, Fuel.Hybrid };
            FuelTypeComboBox.ItemsSource = fuelTypes;
            List<Transmission> transmissionTypes = new List<Transmission> { Transmission.Automatic, Transmission.Manual };
            TransmissionTypeComboBox.ItemsSource = transmissionTypes;
            List<CarType> carTypes = new List<CarType> { CarType.Economy, CarType.Business, CarType.Coupe, CarType.Limousine };
            CarTypeComboBox.ItemsSource = carTypes;
            List<Brand> brands = new List<Brand> { Brand.Audi, Brand.Bentley, Brand.BMW, Brand.Chevrolet, Brand.Ford, Brand.Honda, Brand.Hyundai, Brand.Jaguar, Brand.Kia, Brand.Lamborghini, Brand.Lexus, Brand.Mazda, Brand.MercedesBenz, Brand.Nissan, Brand.Porsche, Brand.Subaru, Brand.Tesla, Brand.Toyota, Brand.Volkswagen };
            BrandComboBox.ItemsSource = brands;

            FuelTypeComboBox.SelectedIndex = 0;
            CarTypeComboBox.SelectedIndex = 0;
            TransmissionTypeComboBox.SelectedIndex = 0;
            BrandComboBox.SelectedIndex = 0;

            FuelTypeComboBox.SelectedValue = null;
            CarTypeComboBox.SelectedValue = null;
            TransmissionTypeComboBox.SelectedValue = null;
            BrandComboBox.SelectedValue = null;
        }

        public void ShowCarsInDataGrid(DataGrid carsGrid, CarParkManager carParkManager)
        {
            carParkManager.manager.OpenConnection();

            string selectQuery = "SELECT * FROM Cars";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, carParkManager.manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);

                    carsGrid.ItemsSource = dataTable.DefaultView;
                }
            }

            carParkManager.manager.CloseConnection();
        }

        private void CarAddButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string number = NumberTextBox.Text;
            Fuel selectedFuel;
            Transmission selectedTransmission;
            CarType selectedCarType;
            Brand selectedBrand;

            if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(number)
                && Enum.TryParse<Fuel>(FuelTypeComboBox.SelectedValue?.ToString(), out selectedFuel) && Enum.TryParse<Transmission>(TransmissionTypeComboBox.SelectedValue?.ToString(), out selectedTransmission)
                && Enum.TryParse<CarType>(CarTypeComboBox.SelectedValue?.ToString(), out selectedCarType) && Enum.TryParse<Brand>(BrandComboBox.SelectedValue?.ToString(), out selectedBrand))
            {
                string colour = ColourTextBox.Text;
                int yearOfManufacture;
                int imageID;
                double hourPrice;

                if (int.TryParse(YearOfManufactureTextBox.Text, out yearOfManufacture) && int.TryParse(ImageIDTextBox.Text, out imageID) && double.TryParse(HourPriceTextBox.Text, out hourPrice))
                {
                    if (carParkManager.ValidateDataCar(name, selectedFuel, selectedTransmission, selectedCarType, selectedBrand, colour, yearOfManufacture, number, imageID, hourPrice))
                    {
                        carParkManager.AddCar(name, selectedFuel, selectedTransmission, selectedCarType, selectedBrand, colour, yearOfManufacture, number, imageID, hourPrice);
                        ShowCarsInDataGrid(carsGrid, carParkManager);
                        ClearFieldsAndNotifies();
                    }
                }
                else
                {
                    // Обработка ошибок при некорректном вводе числовых значений
                    MessageBox.Show("Invalid numeric values entered.");
                }
            }
            else
            {
                // Обработка ошибок при некорректном выборе типов топлива или трансмиссии или пустом значении имени
                MessageBox.Show("Invalid fuel type or transmission selected, or empty name field.");
            }

        }

        private void CarDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, есть ли выбранный элемент в DataGrid
            if (carsGrid.SelectedItem != null)
            {
                // Получаем выбранный объект машины из DataGrid
                var rowView = carsGrid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    // Извлекаем значения столбцов из DataRowView
                    string name = rowView["Name"] as string;
                    Int64 fuelValue = (Int64)rowView["FuelType"];
                    Int64 transmissionValue = (Int64)rowView["TransmissionType"];
                    Int64 carTypeValue = (Int64)rowView["CarType"];
                    Int64 brand = (Int64)rowView["Brand"];
                    string colour = rowView["Colour"] as string;
                    Int64 yearOfManufacture = (Int64)rowView["YearOfManufacture"];
                    string number = rowView["Number"] as string;
                    Int64 imageID = (Int64)rowView["ImageID"];
                    double hourPrice = Convert.ToDouble(rowView["HourPrice"]);

                    // Преобразуем целочисленное значение fuelValue в перечисление Fuel
                    Fuel selectedFuel = (Fuel)fuelValue;

                    // Преобразуем целочисленное значение transmissionValue в перечисление Transmission
                    Transmission selectedTransmission = (Transmission)transmissionValue;

                    CarType selectedCarType = (CarType)carTypeValue;

                    Brand selectedBrand = (Brand)brand;

                    // Создаем объект Car
                    Car selectedCar = new Car(name, selectedFuel, selectedTransmission, selectedCarType, selectedBrand, colour, (int)yearOfManufacture, number, (int)imageID, hourPrice);

                    // Удаляем машину из списка carParkManager
                    carParkManager.RemoveCar(selectedCar);

                    // Перезагружаем DataGrid для обновления отображения
                    ShowCarsInDataGrid(carsGrid, carParkManager);
                }
            }
            else
            {
                // Если не выбрана ни одна машина, выводим сообщение
                MessageBox.Show("No car selected.");
            }
        }

        private void CarEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (carsGrid.SelectedItem != null)
            {
                var rowView = carsGrid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    // Извлекаем значения столбцов из DataRowView
                    string name = rowView["Name"] as string;
                    Int64 fuelValue = (Int64)rowView["FuelType"];
                    Int64 transmissionValue = (Int64)rowView["TransmissionType"];
                    Int64 carTypeValue = (Int64)rowView["CarType"];
                    Int64 brandValue = (Int64)rowView["Brand"];
                    string colour = rowView["Colour"] as string;
                    Int64 yearOfManufacture = (Int64)rowView["YearOfManufacture"];
                    string number = rowView["Number"] as string;
                    Int64 imageID = (Int64)rowView["ImageID"];
                    double hourPrice = Convert.ToDouble(rowView["HourPrice"]);
                    Fuel selectedFuel = (Fuel)fuelValue;
                    Transmission selectedTransmission = (Transmission)transmissionValue;
                    CarType selectedCarType = (CarType)carTypeValue;
                    Brand selectedBrand = (Brand)brandValue;

                    NameTextBox.Text = name;

                    FuelTypeComboBox.SelectedItem = selectedFuel.ToString();
                    TransmissionTypeComboBox.SelectedItem = selectedTransmission.ToString();
                    CarTypeComboBox.SelectedItem = selectedCarType.ToString();
                    BrandComboBox.SelectedItem = selectedBrand.ToString();

                    ColourTextBox.Text = colour;
                    YearOfManufactureTextBox.Text = yearOfManufacture.ToString();
                    NumberTextBox.Text = number;
                    ImageIDTextBox.Text = imageID.ToString();
                    HourPriceTextBox.Text = hourPrice.ToString();

                    isEditMode = true;
                    lastEditableCarID = number;
                }
            }
            else
            {
                ClearFields();
            }
        }

        private void CarSaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            var rowView = carsGrid.SelectedItem as DataRowView;
            if (isEditMode == true && lastEditableCarID == rowView["Number"] as string)
            {
                if (!ValidateFields())
                {
                    return;
                }

                Int64 ID = 0;
                if (rowView != null)
                {
                    ID = (Int64)rowView["ID"];
                }

                string colour = ColourTextBox.Text;
                string name = NameTextBox.Text;
                Fuel selectedFuel;
                Transmission selectedTransmission;
                CarType selectedCarType;
                Brand brand;

                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(colour) &&
                    Enum.TryParse<Fuel>(FuelTypeComboBox.SelectedValue?.ToString(), out selectedFuel) &&
                    Enum.TryParse<Transmission>(TransmissionTypeComboBox.SelectedValue?.ToString(), out selectedTransmission) &&
                    Enum.TryParse<CarType>(CarTypeComboBox.SelectedValue?.ToString(), out selectedCarType) &&
                    Enum.TryParse<Brand>(BrandComboBox.SelectedValue?.ToString(), out brand))
                {
                    int yearOfManufacture;
                    string number = NumberTextBox.Text;
                    int imageID;
                    double hourPrice;

                    if (int.TryParse(YearOfManufactureTextBox.Text, out yearOfManufacture) &&
                        int.TryParse(ImageIDTextBox.Text, out imageID) && double.TryParse(HourPriceTextBox.Text, out hourPrice))
                    {
                        string updateQuery = "UPDATE Cars SET Name = @Name, FuelType = @FuelType, TransmissionType = @TransmissionType, CarType = @CarType, Brand = @Brand, Colour = @Colour, YearOfManufacture = @YearOfManufacture, Number = @Number, ImageID = @ImageID, HourPrice = @HourPrice WHERE ID = @ID";

                        carParkManager.manager.OpenConnection();

                        using (SQLiteCommand command = new SQLiteCommand(updateQuery, carParkManager.manager.Connection))
                        {
                            command.Parameters.AddWithValue("@Name", name);
                            command.Parameters.AddWithValue("@FuelType", (int)selectedFuel);
                            command.Parameters.AddWithValue("@TransmissionType", (int)selectedTransmission);
                            command.Parameters.AddWithValue("@CarType", (int)selectedCarType);
                            command.Parameters.AddWithValue("@Brand", (int)brand);
                            command.Parameters.AddWithValue("@Colour", colour);
                            command.Parameters.AddWithValue("@YearOfManufacture", yearOfManufacture);
                            command.Parameters.AddWithValue("@Number", number);
                            command.Parameters.AddWithValue("@ImageID", imageID);
                            command.Parameters.AddWithValue("@HourPrice", hourPrice);
                            command.Parameters.AddWithValue("@ID", ID);

                            command.ExecuteNonQuery();
                        }
                        string selectQuery = "SELECT ID FROM Cars WHERE Name = @Name AND FuelType = @FuelType AND TransmissionType = @TransmissionType AND CarType = @CarType AND Brand = @Brand AND Colour = @Colour AND YearOfManufacture = @YearOfManufacture AND Number = @Number AND ImageID = @ImageID AND HourPrice = @HourPrice";
                        int carId;

                        using (SQLiteCommand selectCommand = new SQLiteCommand(selectQuery, carParkManager.manager.Connection))
                        {
                            selectCommand.Parameters.AddWithValue("@Name", name);
                            selectCommand.Parameters.AddWithValue("@FuelType", (int)selectedFuel);
                            selectCommand.Parameters.AddWithValue("@TransmissionType", (int)selectedTransmission);
                            selectCommand.Parameters.AddWithValue("@CarType", (int)selectedCarType);
                            selectCommand.Parameters.AddWithValue("@Brand", (int)brand);
                            selectCommand.Parameters.AddWithValue("@Colour", colour);
                            selectCommand.Parameters.AddWithValue("@YearOfManufacture", yearOfManufacture);
                            selectCommand.Parameters.AddWithValue("@Number", number);
                            selectCommand.Parameters.AddWithValue("@ImageID", imageID);
                            selectCommand.Parameters.AddWithValue("@HourPrice", hourPrice);

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

                        if (selectedCarType == CarType.Economy)
                        {
                            carClass = "EconomyClass";
                        }
                        else if (selectedCarType == CarType.Business)
                        {
                            carClass = "BusinessClass";
                        }
                        else if (selectedCarType == CarType.Coupe)
                        {
                            carClass = "CoupeClass";
                        }
                        else if (selectedCarType == CarType.Limousine)
                        {
                            carClass = "LimousineClass";
                        }

                        string updateCarFromClass = $"UPDATE {carClass} SET FuelType = @FuelType, TransmissionType = @TransmissionType, Brand = @Brand, Colour = @Colour, Number = @Number WHERE CarID = @CarID";

                        using (SQLiteCommand commandCarClass = new SQLiteCommand(updateCarFromClass, carParkManager.manager.Connection))
                        {
                            commandCarClass.Parameters.AddWithValue("@FuelType", (int)selectedFuel);
                            commandCarClass.Parameters.AddWithValue("@TransmissionType", (int)selectedTransmission);
                            commandCarClass.Parameters.AddWithValue("@Brand", (int)brand);
                            commandCarClass.Parameters.AddWithValue("@Colour", colour);
                            commandCarClass.Parameters.AddWithValue("@Number", number);
                            commandCarClass.Parameters.AddWithValue("@CarID", carId);
                            commandCarClass.ExecuteNonQuery();
                        }
                    }

                    carParkManager.manager.CloseConnection();
                }
                else
                {
                    // Обработка ошибок при некорректном вводе числовых значений
                    MessageBox.Show("Invalid numeric values entered.");
                }
            }
            else
            {
                // Обработка ошибок при некорректном выборе типов топлива или трансмиссии или пустом значении имени
                MessageBox.Show("Invalid fuel type or transmission selected, or empty name field.");
            }
            isEditMode = false;
            ClearFields();
            ShowCarsInDataGrid(carsGrid, carParkManager);
            ClearFields();
        }
        private bool ValidateFields()
        {
            string name = NameTextBox.Text;
            string colour = ColourTextBox.Text;
            string number = NumberTextBox.Text;
            Fuel selectedFuel;
            Transmission selectedTransmission;
            CarType selectedCarType;
            Brand selectedCarBrand;
            int yearOfManufacture;
            int imageID;
            double hourPrice;

            bool isValid = true;

            // Validate name, fuel type, transmission type, car type, and brand
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(number) ||
                !Enum.TryParse<Fuel>(FuelTypeComboBox.SelectedValue?.ToString(), out selectedFuel) ||
                !Enum.TryParse<Transmission>(TransmissionTypeComboBox.SelectedValue?.ToString(), out selectedTransmission) ||
                !Enum.TryParse<CarType>(CarTypeComboBox.SelectedValue?.ToString(), out selectedCarType) ||
                !Enum.TryParse<Brand>(BrandComboBox.SelectedValue?.ToString(), out selectedCarBrand))
            {
                NameNotify.Foreground = new SolidColorBrush(Colors.Red);
                NameNotify.Content = "Поле должно быть заполнено";
                ColourNotify.Foreground = new SolidColorBrush(Colors.Red);
                ColourNotify.Content = "Поле должно быть заполнено";
                NumberNotify.Foreground = new SolidColorBrush(Colors.Red);
                NumberNotify.Content = "Поле должно быть заполнено";
                FuelTypeNotify.Foreground = new SolidColorBrush(Colors.Red);
                FuelTypeNotify.Content = "Поле должно быть заполнено";
                TransmissionTypeNotify.Foreground = new SolidColorBrush(Colors.Red);
                TransmissionTypeNotify.Content = "Поле должно быть заполнено";
                CarTypeNotify.Foreground = new SolidColorBrush(Colors.Red);
                CarTypeNotify.Content = "Поле должно быть заполнено";
                BrandNotify.Foreground = new SolidColorBrush(Colors.Red);
                BrandNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                NameNotify.Content = "";
                ColourNotify.Content = "";
                NumberNotify.Content = "";
                FuelTypeNotify.Content = "";
                TransmissionTypeNotify.Content = "";
                CarTypeNotify.Content = "";
                BrandNotify.Content = "";
            }

            // Validate numeric values
            if (!int.TryParse(YearOfManufactureTextBox.Text.ToString(), out yearOfManufacture) ||
             !int.TryParse(ImageIDTextBox.Text.ToString(), out imageID) ||
             !double.TryParse(HourPriceTextBox.Text, out hourPrice) || string.IsNullOrWhiteSpace(HourPriceTextBox.Text))
            {
                YearOfManufactureNotify.Foreground = new SolidColorBrush(Colors.Red);
                YearOfManufactureNotify.Content = "Поле должно быть заполнено";
                ImageNotify.Foreground = new SolidColorBrush(Colors.Red);
                ImageNotify.Content = "Поле должно быть заполнено";
                HourPriceNotify.Foreground = new SolidColorBrush(Colors.Red);
                HourPriceNotify.Content = "Поле должно быть заполнено";
                isValid = false;
            }
            else
            {
                ClearNotifies();
            }

            return isValid;
        }

        private void ClearFields()
        {
            NameTextBox.Text = "";
            FuelTypeComboBox.SelectedItem = null;
            TransmissionTypeComboBox.SelectedItem = null;
            CarTypeComboBox.SelectedItem = null;
            BrandComboBox.SelectedItem = null;
            ColourTextBox.Text = "";
            YearOfManufactureTextBox.Text = null;
            NumberTextBox.Text = null;
            ImageIDTextBox.Text = null;
            HourPriceTextBox.Text = null;
        }
        public void ClearNotifies()
        {
            NameNotify.Content = "";
            FuelTypeNotify.Content = "";
            TransmissionTypeNotify.Content = "";
            CarTypeNotify.Content = "";
            BrandNotify.Content = "";
            ColourNotify.Content = "";
            YearOfManufactureNotify.Content = "";
            NumberNotify.Content = "";
            ImageNotify.Content = "";
            HourPriceNotify.Content = "";
        }
        public void ClearFieldsAndNotifies()
        {
            NameTextBox.Text = "";
            FuelTypeComboBox.SelectedItem = null;
            TransmissionTypeComboBox.SelectedItem = null;
            CarTypeComboBox.SelectedItem = null;
            BrandComboBox.SelectedItem = null;
            ColourTextBox.Text = "";
            YearOfManufactureTextBox.Text = null;
            NumberTextBox.Text = null;
            ImageIDTextBox.Text = null;
            HourPriceTextBox.Text = null;

            NameNotify.Content = "";
            FuelTypeNotify.Content = "";
            TransmissionTypeNotify.Content = "";
            CarTypeNotify.Content = "";
            BrandNotify.Content = "";
            ColourNotify.Content = "";
            YearOfManufactureNotify.Content = "";
            NumberNotify.Content = "";
            ImageNotify.Content = "";
            HourPriceNotify.Content = "";
        }

        private void CarClearFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFieldsAndNotifies();
        }

        private void CarClearTableButton_Click(object sender, RoutedEventArgs e)
        {
            carParkManager.ClearCarsTable();
            ShowCarsInDataGrid(carsGrid,carParkManager);
        }
        private void FuelTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> fuelTypes = new List<string> { "Petrol", "Diesel", "Gas", "Electricity" };
            FuelTypeComboBox.ItemsSource = fuelTypes;
        }

        private void TransmissionTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> transmissionTypes = new List<string> { "Automatic", "Manual" };
            TransmissionTypeComboBox.ItemsSource = transmissionTypes;
        }
        private void CarTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> carTypes = new List<string> { "Economy",
            "Business",
            "Coupe",
            "Limousine"};
            CarTypeComboBox.ItemsSource = carTypes;
        }
        private void BrandComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<string> brandTypes = new List<string> { "Toyota",
            "Honda",
            "Ford",
            "BMW",
            "MercedesBenz",
            "Audi",
            "Volkswagen",
            "Tesla",
            "Porsche",
            "Hyundai",
            "Chevrolet",
            "Nissan",
            "Jaguar",
            "Lexus",
            "Subaru",
            "Mazda",
            "Kia",
            "Bentley",
            "Lamborghini"
            };
            BrandComboBox.ItemsSource = brandTypes;
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ToggleMaximizeOnDoubleClick(e);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragWithLeftButton(e);
        }

        private void WindowHide_Click(object sender, RoutedEventArgs e)
        {
            this.Minimize();
        }

        private void WindowOpenFull_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleMaximize();
        }

        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            WindowExtensions.ExitApplication();
        }

        //bar

        private void CarParkButton_Click(object sender, RoutedEventArgs e)
        {
            AdminCarParkWindow window = new AdminCarParkWindow();
            window.Show();
            this.Close();
        }

        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            AdminRentWindow window = new AdminRentWindow();
            window.Show();
            this.Close();
        }

        private void CarParkEditingButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RentalTrackingButton_Click(object sender, RoutedEventArgs e)
        {
            AdminRentalTracking window = new AdminRentalTracking();
            window.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow window = new SignInWindow();
            window.Show();
            this.Hide();
        }
    }
}
