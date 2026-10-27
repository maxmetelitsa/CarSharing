using System.Windows;
using System.Windows.Input;
using CarSharing.Authorization;
using CarSharing.CarPark;
using CarSharing.CarsInfo;
using CarSharing.Data;
using CarSharing.Functions;
using CarSharing.Profile;
using CarSharing.Services;
using CarSharing.UsersInfo;
using CarSharing.ViewModels;

namespace CarSharing.Rent
{
    public partial class RentCarWindow : Window
    {
        public RentCarWindow(Car car)
        {
            InitializeComponent();

            // Re-read the car so its ID and price come straight from the database.
            Car freshCar = new CarRepository().GetById(car.ID) ?? car;

            var viewModel = new RentCarViewModel(
                freshCar,
                CurrentUserManager.CurrentUser,
                new RentalRepository(),
                new ContractService(),
                new MessageBoxService());

            viewModel.BookingCompleted += OnBookingCompleted;
            DataContext = viewModel;
        }

        private void OnBookingCompleted()
        {
            new ProfileWindow().Show();
            Hide();
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