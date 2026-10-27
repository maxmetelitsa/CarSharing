using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CarSharing.Enums;

namespace CarSharing.CarsInfo
{
    public class Car
    {
        private static int lastID = 0;
        public int ID { get; set; }
        private string name;
        private Fuel fuelType;
        private Transmission transmissionType;
        private CarType carType;
        private Brand brand;
        private string colour;
        private int yearOFManufacture;
        private string number;
        private int imageId;
        private double hourPrice;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public Fuel FuelType
        {
            get { return fuelType; }
            set { fuelType = value; }
        }
        public Transmission TransmissionType
        {
            get { return transmissionType; }
            set { transmissionType = value; }
        }
        public CarType CarType
        {
            get { return carType; }
            set { carType = value; }
        }
        public Brand Brand
        {
            get { return brand; }
            set { brand = value; }
        }
        public string Colour
        {
            get { return colour; }
            set { colour = value; }
        }
        public int YearOfManufacture
        {
            get { return yearOFManufacture; }
            set { yearOFManufacture = value; }
        }
        public string Number
        {
            get { return number; }
            set { number = value; }
        }
        public int ImageID
        {
            get { return imageId; }
            set { imageId = value; }
        }
        public double DayPrice
        {
            get { return hourPrice; }
            set { hourPrice = value; }
        }
        public Car(string name, Fuel fuelType, Transmission transmissionType, CarType carType, Brand brand, string colour, int yearOfManufacture, string number, int imageId, double hourPrice)
        {
            ID = ++lastID;
            Name = name;
            FuelType = fuelType;
            TransmissionType = transmissionType;
            CarType = carType;
            Brand = brand;
            Colour = colour;
            YearOfManufacture = yearOfManufacture;
            Number = number;
            ImageID = imageId;
            DayPrice = hourPrice;
        }
        public Car(int id, string name, Fuel fuelType, Transmission transmissionType, CarType carType, Brand brand, string colour, int yearOfManufacture, string number, int imageId, double hourPrice)
        {
            ID = id;
            Name = name;
            FuelType = fuelType;
            TransmissionType = transmissionType;
            CarType = carType;
            Brand = brand;
            Colour = colour;
            YearOfManufacture = yearOfManufacture;
            Number = number;
            ImageID = imageId;
            DayPrice = hourPrice;
        }
    }
}
