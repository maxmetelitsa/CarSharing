using CarSharing.Functions;
using CarSharing.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static CarSharing.Functions.Functions;
using CarSharing.UsersInfo;
using CarSharing.Profile;

namespace CarSharing.Authorization
{
    /// <summary>
    /// Логика взаимодействия для SignUpWindowNew.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        SQLiteManager manager = new SQLiteManager();
        Users usersList;
        public SignUpWindow()
        {
            usersList = new Users(manager);
            InitializeComponent();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.ToggleMaximizeOnDoubleClick(e);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragWithLeftButton(e);
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            SignInWindow window = new SignInWindow();
            window.Show();
            this.Close();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (isCorrectRegistration() == true)
            {
                if (usersList.AddUser(manager, TextBoxName.Text, TextBoxSurname.Text, TextBoxEmail.Text, TextBoxPassword.Text) == true)
                {
                    SignInWindow window = new SignInWindow();
                    window.Show();
                    this.Close();
                }
            }
        }

        private bool isCorrectRegistration()
        {
            string firstName = TextBoxName.Text;
            string lastName = TextBoxSurname.Text;
            string email = TextBoxEmail.Text;
            string password = TextBoxPassword.Text;
            string confirmPassword = TextBoxPasswordRepeat.Text;
            bool flag = true;
            string clear = "";

            if (string.IsNullOrEmpty(firstName))
            {
                TextBoxNameNotify.Foreground = new SolidColorBrush(Colors.Red);
                TextBoxNameNotify.Content = "Поле должно быть заполнено";
                flag = false;
            }
            else
            {
                TextBoxNameNotify.Content = clear;
            }

            if (string.IsNullOrEmpty(lastName))
            {
                TextBoxSurnameNotify.Foreground = new SolidColorBrush(Colors.Red);
                TextBoxSurnameNotify.Content = "Поле должно быть заполнено";
                flag = false;
            }
            else
            {
                TextBoxSurnameNotify.Content = clear;
            }

            bool isEmailAlreadyUsed = false;

            foreach (User user in usersList.users)
            {
                if (user.Email == email)
                {
                    TextBoxEmailNotify.Foreground = new SolidColorBrush(Colors.Red);
                    TextBoxEmailNotify.Content = "Данный email уже используется";
                    isEmailAlreadyUsed = true;
                    flag = false;
                    break;
                }
            }

            if (!isEmailAlreadyUsed)
            {
                if (string.IsNullOrEmpty(email))
                {
                    TextBoxEmailNotify.Foreground = new SolidColorBrush(Colors.Red);
                    TextBoxEmailNotify.Content = "Поле должно быть заполнено";
                    flag = false;
                }
                else
                {
                    if (!IsValidEmail(email))
                    {
                        TextBoxEmailNotify.Foreground = new SolidColorBrush(Colors.Red);
                        TextBoxEmailNotify.Content = "Некорректный формат email";
                        flag = false;
                    }
                    else
                    {
                        TextBoxEmailNotify.Content = clear;
                    }
                }
            }

            if (string.IsNullOrEmpty(password))
            {
                TextBoxPasswordNotify.Foreground = new SolidColorBrush(Colors.Red);
                TextBoxPasswordNotify.Content = "Поле должно быть заполнено";
                flag = false;
            }
            else
            {
                TextBoxPasswordNotify.Content = clear;
            }

            if (string.IsNullOrEmpty(confirmPassword))
            {
                TextBoxPasswordRepeatNotify.Foreground = new SolidColorBrush(Colors.Red);
                TextBoxPasswordRepeatNotify.Content = "Поле должно быть заполнено";
                flag = false;
            }
            else if (password != confirmPassword)
            {
                TextBoxPasswordRepeatNotify.Foreground = new SolidColorBrush(Colors.Red);
                TextBoxPasswordRepeatNotify.Content = "Пароли не совпадают";
                flag = false;
            }
            else
            {
                TextBoxPasswordRepeatNotify.Content = clear;
            }

            return flag;
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
    }
}