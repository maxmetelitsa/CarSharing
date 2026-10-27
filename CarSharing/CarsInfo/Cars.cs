using CarSharing.Enums;
using CarSharing.SQL_Manager;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Windows.Media;
using CarSharing.CarPark;
using System;

namespace CarSharing.CarsInfo
{
    public class Cars
    {
        public List<Car> cars;

        public Cars()
        {
            cars = new List<Car>(); 
        }
    }
}
