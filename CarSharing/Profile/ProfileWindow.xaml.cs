using CarSharing.Services;
using CarSharing.Security;
using CarSharing.Functions;
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
using CarSharing.CarPark;
using CarSharing.Rent;
using CarSharing.Profile;
using CarSharing.About;
using CarSharing.UsersInfo;
using CarSharing.Enums;
using System.Data.SQLite;
using CarSharing.SQL_Manager;
using CarSharing.Authorization;
using CarSharing.CarsInfo;
using CarSharing.OrdersInfo;
using CarSharing.UserOrders;
using System.Diagnostics;

namespace CarSharing.Profile
{
    /// <summary>
    /// Логика взаимодействия для TestMainWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        public SQLiteManager manager;
        public OrdersManager ordersManager;
        public UserOrderManager userOrdersManager;
        public ProfileWindow()
        {
            manager = new SQLiteManager();
            ordersManager = new OrdersManager();
            userOrdersManager = new UserOrderManager();
            CurrentUser currentUserData = CurrentUserManager.CurrentUser;
            userOrdersManager.userOrdersList.GetUserOrders(currentUserData.ID);
            InitializeComponent();
            ShowInfo();
        }
        private void ShowInfo()
        {
            SetReadAbleFields();

            CurrentUser currentUserData = CurrentUserManager.CurrentUser;

            manager.OpenConnection();

            string selectQuery = $"SELECT * FROM Users WHERE ID = @ID";

            using (SQLiteCommand command = new SQLiteCommand(selectQuery, manager.Connection))
            {
                command.Parameters.AddWithValue("@ID", currentUserData.ID);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string nameFromDB = reader["Name"].ToString();
                        string surnameFromDB = reader["Surname"].ToString();
                        string emailFromDB = reader["Email"].ToString();
                        string passwordFromDB = reader["Password"].ToString();
                        string passportNumberFromDB = reader["PassportNumber"].ToString();
                        string identificationNumberFromDB = reader["IdentificationNumber"].ToString();
                        string licenseSeriesFromDB = reader["LicenseSeries"].ToString();
                        string licenseNumberFromDB = reader["LicenseNumber"].ToString();

                        currentUserData.Name = nameFromDB;
                        currentUserData.Surname = surnameFromDB;
                        currentUserData.Email = emailFromDB;
                        currentUserData.Password = passwordFromDB;
                        currentUserData.PassportNumber = passportNumberFromDB;
                        currentUserData.IdentificationNumber = identificationNumberFromDB;
                        currentUserData.LicenseSeries = licenseSeriesFromDB;
                        currentUserData.LicenseNumber = licenseNumberFromDB;

                        NameTextBox.Text = nameFromDB;
                        SurnameTextBox.Text = surnameFromDB;
                        EmailTextBox.Text = emailFromDB;
                        // Only the password hash is stored, so the fields stay empty
                        // until the user types a new password.
                        PasswordTextBox.Text = string.Empty;
                        PasswordRepeatTextBox.Text = string.Empty;
                        PassportNumberTextBox.Text = passportNumberFromDB;
                        IdentificationTextBox.Text = identificationNumberFromDB;
                        LicenseSeriesTextBox.Text = licenseSeriesFromDB;
                        LicenseNumberTextBox.Text = licenseNumberFromDB;
                    }
                }
            }

            ShowUserOrders();
            manager.CloseConnection();
        }
        public void ShowUserOrders()
        {
            RentalsListBox.ItemsSource = userOrdersManager.userOrdersList.currentUserOrders;

            var stackPanel = FindDescendantByName<StackPanel>(RentalsListBox, "BookingStackPanel");
            var bookingTextBlock = FindDescendantByName<TextBlock>(stackPanel, "BookingTextBlock");

            if (bookingTextBlock != null)
            {
                string bookingText = "Бронирование";

                foreach (UserOrder userOrder in userOrdersManager.userOrdersList.currentUserOrders)
                {
                    bookingText = $"Бронирование {userOrder.ID}";
                }

                bookingTextBlock.Text = bookingText;
            }
        }

        public static T FindDescendantByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            if (parent == null)
                return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T namedChild && (namedChild as FrameworkElement)?.Name == name)
                {
                    foundChild = namedChild;
                    break;
                }

                foundChild = FindDescendantByName<T>(child, name);
                if (foundChild != null)
                    break;
            }

            return foundChild;
        }

        public void UpdateUserInfo(string name, string surname, string email, string password, string passportNumber, string identificationNumber, string licenseSeries, string licenseNumber)
        {
            if (PasswordTextBox.Text != PasswordRepeatTextBox.Text)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }
            else
            {

                CurrentUser currentUserData = CurrentUserManager.CurrentUser;

                if (ValidateUserFields(name, surname, email))
                {
                    // An empty password field keeps the current password.
                    string passwordHash = string.IsNullOrEmpty(password)
                        ? currentUserData.Password
                        : PasswordHasher.Hash(password);

                    manager.OpenConnection();

                    string updateQuery = "UPDATE Users SET Name = @Name, Surname = @Surname, Email = @Email, Password = @Password, PassportNumber = @PassportNumber, IdentificationNumber = @IdentificationNumber, LicenseSeries = @LicenseSeries, LicenseNumber = @LicenseNumber WHERE ID = @ID";

                    using (SQLiteCommand command = new SQLiteCommand(updateQuery, manager.Connection))
                    {
                        command.Parameters.AddWithValue("@Name", name);
                        command.Parameters.AddWithValue("@Surname", surname);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", passwordHash);
                        command.Parameters.AddWithValue("@PassportNumber", passportNumber);
                        command.Parameters.AddWithValue("@IdentificationNumber", identificationNumber);
                        command.Parameters.AddWithValue("@LicenseSeries", licenseSeries);
                        command.Parameters.AddWithValue("@LicenseNumber", licenseNumber);
                        command.Parameters.AddWithValue("@ID", currentUserData.ID);

                        command.ExecuteNonQuery();
                    }

                    manager.CloseConnection();

                    currentUserData.Name = name;
                    currentUserData.Surname = surname;
                    currentUserData.Email = email;
                    currentUserData.Password = passwordHash;
                    currentUserData.PassportNumber = passportNumber;
                    currentUserData.IdentificationNumber = identificationNumber;
                    currentUserData.LicenseSeries = licenseSeries;
                    currentUserData.LicenseNumber = licenseNumber;
                }
            }
        }

        private void SaveChangesProfileButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string surname = SurnameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordTextBox.Text;
            string passportNumber = PassportNumberTextBox.Text;
            string identificationNumber = IdentificationTextBox.Text;
            string licenseSeries = LicenseSeriesTextBox.Text;
            string licenseNumber = LicenseNumberTextBox.Text;

            UpdateUserInfo(name, surname, email, password, passportNumber, identificationNumber, licenseSeries, licenseNumber);
            ShowInfo();
        }

        private void ClearFields()
        {
            PasswordTextBox.Text = null;
            PasswordRepeatTextBox.Text = null;
            PassportNumberTextBox.Text = null;
            IdentificationTextBox.Text = null;
            LicenseSeriesTextBox.Text = null;
            LicenseNumberTextBox.Text = null;
        }
        public bool ValidateUserFields(string name, string surname, string email)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        private void SetReadAbleFields()
        {
            NameTextBox.IsReadOnly = true;
            SurnameTextBox.IsReadOnly = true;
            EmailTextBox.IsReadOnly = true;
            PasswordTextBox.IsReadOnly = true;
            PasswordRepeatTextBox.IsReadOnly = true;
            PassportNumberTextBox.IsReadOnly = true;
            IdentificationTextBox.IsReadOnly = true;
            LicenseSeriesTextBox.IsReadOnly = true;
            LicenseNumberTextBox.IsReadOnly = true;
        }
        private void SetWriteAbleFields()
        {
            NameTextBox.IsReadOnly = true;
            SurnameTextBox.IsReadOnly = true;
            EmailTextBox.IsReadOnly = true;
            PasswordTextBox.IsReadOnly = false;
            PasswordRepeatTextBox.IsReadOnly = false;
            PassportNumberTextBox.IsReadOnly = false;
            IdentificationTextBox.IsReadOnly = false;
            LicenseSeriesTextBox.IsReadOnly = false;
            LicenseNumberTextBox.IsReadOnly = false;
        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void ClearFieldsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }
        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            SetWriteAbleFields();
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

        private void CarParkButton_Click(object sender, RoutedEventArgs e)
        {
            CarParkWindow window = new CarParkWindow();
            window.Show();
            this.Close();
        }

        private void RentButton_Click(object sender, RoutedEventArgs e)
        {
            RentWindow window = new RentWindow();
            window.Show();
            this.Close();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            //AboutWindow window = new AboutWindow();
            //window.Show();
            //this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow window = new SignInWindow();
            window.Show();
            this.Hide();
        }

        private void RentalsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = RentalsListBox.SelectedItem as UserOrder;

            if (selectedItem != null)
            {
                string filePath = System.IO.Path.Combine(AppPaths.UserRentalsDirectory, $"user_order{selectedItem.ID}.docx");

                try
                {
                    // Получить информацию о каталоге
                    string directoryPath = System.IO.Path.GetDirectoryName(filePath);
                    System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(directoryPath);

                    // Обновить содержимое каталога
                    directoryInfo.Refresh();

                    // Открыть документ
                    Process.Start(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при открытии файла: " + ex.Message);
                }
            }
        }

    }
}