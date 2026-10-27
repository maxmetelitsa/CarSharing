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

namespace CarSharing.About
{
    /// <summary>
    /// Логика взаимодействия для TestMainWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
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
            this.Close();
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
            
        }

        private void AboutCompany_Loaded(object sender, RoutedEventArgs e)
        {
            //string text = "Мы - компания Car House, являемся надежным брендом в сфере проката автомобилей в Беларуси. " +
            //    "Наш приоритет - уважение и любовь к нашим клиентам. Мы искренне ценим ваше время и делаем все возможное, чтобы его сэкономить." +
            //    "\r\n\r\nДля вашего удобства мы предлагаем доставку автомобиля в удобное для вас место. Мы стремимся сократить время, затраченное на оформление договора проката." +
            //    " Если вы предоставите нам свои данные при бронировании, мы с радостью заполним договор, чтобы вам осталось только подписать его и забрать ключи от автомобиля.\r\n\r" +
            //    "\nМы гордимся тем, что можем предложить вам надежные автомобили, которые всегда находятся в отличном техническом состоянии. " +
            //    "Мы уделяем внимание каждой детали и гарантируем безопасность и комфорт во время вашей поездки.\r\n\r\n" +
            //    "Car House - это компания, которая стремится обеспечить вам положительный опыт проката автомобиля. " +
            //    "Мы предлагаем качественный сервис, высокий уровень обслуживания и удобство во всех этапах сотрудничества с нами.\r\n\r\n" +
            //    "Выбирая Car House, вы можете быть уверены в надежности и профессионализме нашей компании. Мы готовы предоставить вам надежное транспортное средство и сделать ваше путешествие комфортным и безопасным.";
            //AboutCompanyTextBlock.Text = text;
        }
    }
}