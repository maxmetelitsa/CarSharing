using System.Windows;
using System.Windows.Input;
using CarSharing.About;
using CarSharing.Authorization;
using CarSharing.CarPark;
using CarSharing.Data;
using CarSharing.Functions;
using CarSharing.Profile;
using CarSharing.Services;
using CarSharing.ViewModels;

namespace CarSharing.Rent
{
    public partial class RentWindow : Window
    {
        public RentWindow()
        {
            InitializeComponent();
            DataContext = new CarCatalogViewModel(new CarRepository());
        }

        private void CarCard_Click(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is CarCardViewModel card)
            {
                new RentCarWindow(card.Car).Show();
                Hide();
            }
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
           
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ProfileWindow window = new ProfileWindow();
            window.Show();
            this.Close();
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
    }
}
