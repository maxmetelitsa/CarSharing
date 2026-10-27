using System;
using CarSharing.CarsInfo;
using CarSharing.Services;

namespace CarSharing.ViewModels
{
    /// <summary>What a car card shows: photo, model, class, fuel, gearbox, colour, year, plate and price.</summary>
    public class CarCardViewModel
    {
        public CarCardViewModel(Car car)
        {
            Car = car ?? throw new ArgumentNullException(nameof(car));
        }

        public Car Car { get; }

        public string Name => Car.Name;

        public string Brand => Car.Brand.ToString();

        public string CarClass => Car.CarType.ToString();

        public string Fuel => Car.FuelType.ToString();

        public string Transmission => Car.TransmissionType.ToString();

        public string Colour => Car.Colour;

        public int Year => Car.YearOfManufacture;

        public string Number => Car.Number;

        public double DayPrice => Car.DayPrice;

        public string ImagePath => AppPaths.GetImagePath(Car.ImageID);
    }
}
