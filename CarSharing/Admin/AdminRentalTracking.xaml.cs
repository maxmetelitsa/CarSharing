using CarSharing.Functions;
using CarSharing.CarPark;
using CarSharing.CarsInfo;
using CarSharing.Enums;
using CarSharing.OrdersInfo;
using CarSharing.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CarSharing.UsersInfo;
using CarSharing.Authorization;

namespace CarSharing.Admin
{
    public partial class AdminRentalTracking : Window
    {
        private OrdersManager ordersManager;
        private bool isEditMode = false;
        private string lastEditableOrderID;

        public AdminRentalTracking()
        {
            InitializeComponent();
            ordersManager = new OrdersManager();
            ordersManager.GetAllOrders();
            ShowOrdersInDataGrid(ordersGrid, ordersManager);
        }

        public void ShowOrdersInDataGrid(DataGrid ordersGrid, OrdersManager ordersManager)
        {
            ordersManager.manager.OpenConnection();

            string selectQuery = "SELECT * FROM [Order]";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, ordersManager.manager.Connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    dataTable.Load(reader);

                    ordersGrid.ItemsSource = dataTable.DefaultView;
                }
            }

            ordersManager.manager.CloseConnection();
        }

        private void OrderDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ordersGrid.SelectedItem != null)
            {
                DataRowView rowView = ordersGrid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    // Получение необходимых данных из выбранной строки в ordersGrid
                    Int64 ID = Convert.ToInt64(rowView["ID"]);
                    Int64 userID = Convert.ToInt64(rowView["UserID"]);
                    Int64 carID = Convert.ToInt64(rowView["CarID"]);

                    string startDateText = rowView["StartDate"].ToString();
                    string endDateText = rowView["EndDate"].ToString();

                    DateTime startDate;
                    DateTime endDate;

                    if (DateTime.TryParse(startDateText, out startDate) && DateTime.TryParse(endDateText, out endDate))
                    {
                        double totalPrice = Convert.ToDouble(rowView["TotalPrice"]);

                        // Создание объекта Order на основе полученных данных
                        OrdersInfo.Order selectedOrder = new OrdersInfo.Order((int)userID, (int)carID, startDate, endDate, totalPrice);

                        // Удаление заказа
                        ordersManager.RemoveOrder(selectedOrder, (int)ID);

                        // Обновление отображения в ordersGrid
                        ShowOrdersInDataGrid(ordersGrid, ordersManager);
                    }
                    else
                    {
                        MessageBox.Show("Invalid date format.");
                    }
                }
            }
            else
            {
                MessageBox.Show("No order selected.");
            }
        }

        private void OrderEditButton_Click(object sender, RoutedEventArgs e)
        {
            if (ordersGrid.SelectedItem != null)
            {
                DataRowView rowView = ordersGrid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    Int64 userID = Convert.ToInt64(rowView["UserID"]);
                    Int64 carID = Convert.ToInt64(rowView["CarID"]);

                    string startDateText = rowView["StartDate"].ToString();
                    string endDateText = rowView["EndDate"].ToString();

                    DateTime startDate = DateTime.Parse(startDateText);
                    DateTime endDate = DateTime.Parse(endDateText);

                    double totalPrice = Convert.ToDouble(rowView["TotalPrice"]);

                    UserIDTextBox.Text = userID.ToString();
                    CarIDTextBox.Text = carID.ToString();
                    StartDatePicker.Text = startDate.ToString();
                    EndDatePicker.Text = endDate.ToString();
                    TotalPriceTextBox.Text = totalPrice.ToString();

                    isEditMode = true;
                    lastEditableOrderID = userID.ToString();
                }
            }
            else
            {
                MessageBox.Show("No order selected.");
            }
        }

        private void OrderSaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                int userID;
                int carID;
                DateTime startDate;
                DateTime endDate;
                double totalPrice;

                if (!int.TryParse(UserIDTextBox.Text, out userID))
                {
                    MessageBox.Show("Invalid user ID.");
                    return;
                }

                if (!int.TryParse(CarIDTextBox.Text, out carID))
                {
                    MessageBox.Show("Invalid car ID.");
                    return;
                }

                if (!DateTime.TryParse(StartDatePicker.Text, out startDate))
                {
                    MessageBox.Show("Invalid start date.");
                    return;
                }

                if (!DateTime.TryParse(EndDatePicker.Text, out endDate))
                {
                    MessageBox.Show("Invalid end date.");
                    return;
                }

                if (!double.TryParse(TotalPriceTextBox.Text, out totalPrice))
                {
                    MessageBox.Show("Invalid total price.");
                    return;
                }

                ordersManager.manager.OpenConnection();

                string updateQuery = "UPDATE [Order] SET UserID = @UserID, CarID = @CarID, StartDate = @StartDate, EndDate = @EndDate, TotalPrice = @TotalPrice WHERE ID = @ID";

                using (SQLiteCommand command = new SQLiteCommand(updateQuery, ordersManager.manager.Connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@CarID", carID);
                    command.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    command.Parameters.AddWithValue("@ID", lastEditableOrderID);

                    command.ExecuteNonQuery();
                }

                ordersManager.manager.CloseConnection();

                ClearFieldsAndNotifies();
                isEditMode = false;

                ShowOrdersInDataGrid(ordersGrid, ordersManager);
            }
            else
            {
                MessageBox.Show("No order is being edited.");
            }
        }
        private void OrderAddButton_Click(object sender, RoutedEventArgs e)
        {
            SQLiteManager manager = new SQLiteManager();
            UsersInfo.Users usersList = new UsersInfo.Users(manager);
            CarParkManager parkManager = new CarParkManager();
            parkManager.GetAllCars();
            usersList.GetAllUsers(manager);
            int userID;
            int counter = 0;
            if (!int.TryParse(UserIDTextBox.Text, out userID))
            {
                MessageBox.Show("Invalid user ID.");
                foreach(User user in usersList.users)
                {
                    if(user.ID == userID)
                    {
                        counter = 1;
                    }
                }
                return;
            }
            if(counter == 0)
            {
                MessageBox.Show("Пользователя с таким ID не существует.");
                return;
            }

            int carID;
            if (!int.TryParse(CarIDTextBox.Text, out carID))
            {
                MessageBox.Show("Invalid car ID.");
                return;
            }

            DateTime startDate;
            if (!DateTime.TryParse(StartDatePicker.Text, out startDate))
            {
                MessageBox.Show("Invalid start date.");
                return;
            }

            DateTime endDate;
            if (!DateTime.TryParse(EndDatePicker.Text, out endDate))
            {
                MessageBox.Show("Invalid end date.");
                return;
            }

            double totalPrice;
            if (!double.TryParse(TotalPriceTextBox.Text, out totalPrice))
            {
                MessageBox.Show("Invalid total price.");
                return;
            }

            ordersManager.AddOrder(userID, carID, startDate, endDate, totalPrice);
            ShowOrdersInDataGrid(ordersGrid, ordersManager);
            ClearFieldsAndNotifies();
        }

        private void ShowPrice()
        {
            if (!string.IsNullOrEmpty(CarIDTextBox.Text) && StartDatePicker.SelectedDate != null && EndDatePicker.SelectedDate != null)
            {
                int writtenID;
                if (!int.TryParse(CarIDTextBox.Text, out writtenID))
                {
                    MessageBox.Show("Invalid car ID.");
                    return;
                }
                ordersManager.manager.OpenConnection();

                string selectQuery = "SELECT * FROM [Cars] WHERE ID = @ID";

                using (SQLiteCommand command = new SQLiteCommand(selectQuery, ordersManager.manager.Connection))
                {
                    command.Parameters.AddWithValue("@ID", writtenID);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            double dayPrice = reader.GetDouble(reader.GetOrdinal("HourPrice"));
                            int maxRentalDays = 31;
                            int minRentalDays = 1;

                            DateTime selectedStartDate = StartDatePicker.SelectedDate.GetValueOrDefault();
                            DateTime selectedEndDate = EndDatePicker.SelectedDate.GetValueOrDefault();

                            int days = (selectedEndDate - selectedStartDate).Days;

                            if (days >= maxRentalDays)
                            {
                                MessageBox.Show($"The number of rental days should not exceed {maxRentalDays}.");
                                TotalPriceTextBox.Text = null;
                                DaysTextBox.Text = null;
                            }
                            else if (days < minRentalDays)
                            {
                                MessageBox.Show($"The minimum number of rental days is {minRentalDays}.");
                                TotalPriceTextBox.Text = null;
                                DaysTextBox.Text = null;
                            }
                            else
                            {
                                double total = days * dayPrice;
                                TotalPriceTextBox.Text = total.ToString();
                                DaysTextBox.Text = days.ToString();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Car not found.");
                        }
                    }
                }

                ordersManager.manager.CloseConnection();
            }
        }
        private void ClearFields()
        {
            UserIDTextBox.Text = "";
            CarIDTextBox.Text = "";
            StartDatePicker.Text = "";
            EndDatePicker.Text = "";
            DaysTextBox.Text = "";
            TotalPriceTextBox.Text = "";
        }
        private void ClearNotifies()
        {
            UserIDNotify.Content = null;
            //CarIDNotify.Content = null;
            StartDateNotify.Content = null;
            EndDateNotify.Content = null;
            TotalPriceNotify.Content = null;
        }

        private void ClearFieldsAndNotifies()
        {
            ClearFields();
            MessageBox.Show("Operation completed successfully.");
        }

        private void OrderClearFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
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
            AdminCarParkWindow window = new AdminCarParkWindow();
            window.Show();
            this.Hide();
        }

        private void RentalTrackingButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ToggleMaximizeOnDoubleClick(e);
        }

        private void StartDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowPrice();
        }

        private void EndDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowPrice();
        }

        private void OrderClearTableButton_Click(object sender, RoutedEventArgs e)
        {
            ordersManager.ClearOrdersTable();
            ShowOrdersInDataGrid(ordersGrid, ordersManager);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow window = new SignInWindow();
            window.Show();
            this.Hide();
        }
    }
}
