using System;
using System.Collections.Generic;
using System.Linq;
using CarSharing.CarsInfo;
using CarSharing.Data;
using CarSharing.Domain;
using CarSharing.Enums;
using CarSharing.Services;
using CarSharing.UsersInfo;

namespace CarSharing.Tests.Fakes
{
    internal static class TestData
    {
        public static Car Car(int id, string name, Brand brand, CarType carType, double dayPrice)
        {
            return new Car(id, name, Fuel.Petrol, Transmission.Automatic, carType, brand, "White", 2022, $"A{id:000}AA", id, dayPrice);
        }

        public static CurrentUser Customer(bool withDocuments = true)
        {
            return withDocuments
                ? new CurrentUser(1, "Иван", "Петров", "ivan.petrov@example.com", "hash", "MP1234567", "3010190A001PB1", "9AB", "123456")
                : new CurrentUser(3, "Сергей", "Новик", "sergey.novik@example.com", "hash", "", "", "", "");
        }
    }

    internal class FakeAuthService : IAuthService
    {
        public LoginResult Result { get; set; } = LoginResult.Failed;

        public int Calls { get; private set; }

        public LoginResult SignIn(string email, string password)
        {
            Calls++;
            return Result;
        }
    }

    internal class FakeCarRepository : ICarRepository
    {
        private readonly List<Car> cars;

        public FakeCarRepository(params Car[] cars)
        {
            this.cars = cars.ToList();
        }

        public IReadOnlyList<Car> GetAll() => cars;

        public Car GetById(int id) => cars.FirstOrDefault(c => c.ID == id);
    }

    internal class FakeRentalRepository : IRentalRepository
    {
        public bool Available { get; set; } = true;

        public List<RentalRecord> SavedRentals { get; } = new List<RentalRecord>();

        public int NextOrderId { get; set; } = 7;

        public bool IsAvailable(int carId, DateTime startDate, DateTime endDate) => Available;

        public int SaveRental(RentalRecord rental)
        {
            SavedRentals.Add(rental);
            return NextOrderId;
        }
    }

    internal class FakeContractService : IContractService
    {
        public int CreatedForOrder { get; private set; } = -1;

        public bool Fail { get; set; }

        public void CreateRentalContracts(int orderId, Car car, RentalForm form, RentalQuote quote)
        {
            if (Fail)
            {
                throw new InvalidOperationException("disk is full");
            }

            CreatedForOrder = orderId;
        }
    }

    internal class FakeMessageService : IMessageService
    {
        public List<string> Infos { get; } = new List<string>();

        public List<string> Errors { get; } = new List<string>();

        public void ShowInfo(string message, string title = "") => Infos.Add(message);

        public void ShowError(string message, string title = "") => Errors.Add(message);
    }
}
