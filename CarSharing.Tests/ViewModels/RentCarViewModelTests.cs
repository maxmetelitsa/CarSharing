using System;
using CarSharing.Data;
using CarSharing.Enums;
using CarSharing.Tests.Fakes;
using CarSharing.UsersInfo;
using CarSharing.ViewModels;
using Xunit;

namespace CarSharing.Tests.ViewModels
{
    public class RentCarViewModelTests
    {
        private static readonly DateTime Today = new DateTime(2026, 5, 1, 10, 0, 0);

        private readonly FakeRentalRepository repository = new FakeRentalRepository();
        private readonly FakeContractService contracts = new FakeContractService();
        private readonly FakeMessageService messages = new FakeMessageService();

        private RentCarViewModel CreateViewModel(CurrentUser customer = null, double dayPrice = 120)
        {
            return new RentCarViewModel(
                TestData.Car(41, "Audi A4", Brand.Audi, CarType.Economy, dayPrice),
                customer ?? TestData.Customer(),
                repository,
                contracts,
                messages,
                () => Today);
        }

        private static void ChooseValidPeriod(RentCarViewModel viewModel)
        {
            viewModel.StartDate = Today.Date.AddDays(1);
            viewModel.EndDate = Today.Date.AddDays(4);
        }

        [Fact]
        public void Form_IsFilledFromTheProfileAndReadOnly()
        {
            RentCarViewModel viewModel = CreateViewModel();

            Assert.Equal("Петров", viewModel.Surname);
            Assert.Equal("MP1234567", viewModel.PassportNumber);
            Assert.Equal("9AB", viewModel.LicenseSeries);
            Assert.True(viewModel.IsReadOnly);
        }

        [Fact]
        public void Form_OpensForEditing_WhenCustomerHasNoDocuments()
        {
            RentCarViewModel viewModel = CreateViewModel(TestData.Customer(withDocuments: false));

            Assert.False(viewModel.IsReadOnly);
        }

        [Fact]
        public void EditCommand_UnlocksTheFields()
        {
            RentCarViewModel viewModel = CreateViewModel();

            viewModel.EditCommand.Execute(null);

            Assert.False(viewModel.IsReadOnly);
        }

        [Fact]
        public void ChangingDates_RecalculatesDaysAndTotal()
        {
            RentCarViewModel viewModel = CreateViewModel(dayPrice: 120);

            viewModel.StartDate = new DateTime(2026, 12, 30);
            viewModel.EndDate = new DateTime(2027, 1, 2);

            Assert.Equal("3", viewModel.DaysText);
            Assert.Equal("360", viewModel.TotalText);
        }

        [Fact]
        public void Book_WithMissingDocuments_ShowsErrorsAndDoesNotSave()
        {
            RentCarViewModel viewModel = CreateViewModel(TestData.Customer(withDocuments: false));
            ChooseValidPeriod(viewModel);

            viewModel.Book();

            Assert.Equal("Поле должно быть заполнено", viewModel.LicenseNumberError);
            Assert.Equal("Поле должно быть заполнено", viewModel.PassportNumberError);
            Assert.Empty(repository.SavedRentals);
        }

        [Fact]
        public void Book_WhenCarIsTaken_ShowsErrorAndDoesNotSave()
        {
            repository.Available = false;
            RentCarViewModel viewModel = CreateViewModel();
            ChooseValidPeriod(viewModel);

            viewModel.Book();

            Assert.Empty(repository.SavedRentals);
            Assert.Contains("уже забронирован", Assert.Single(messages.Errors));
        }

        [Fact]
        public void Book_WithValidData_SavesBookingCreatesContractAndCompletes()
        {
            RentCarViewModel viewModel = CreateViewModel(dayPrice: 120);
            ChooseValidPeriod(viewModel);
            bool completed = false;
            viewModel.BookingCompleted += () => completed = true;

            viewModel.BookCommand.Execute(null);

            RentalRecord saved = Assert.Single(repository.SavedRentals);
            Assert.Equal(1, saved.CustomerId);
            Assert.Equal(41, saved.CarId);
            Assert.Equal(360, saved.TotalPrice);
            Assert.Equal("123456", saved.LicenseNumber);
            Assert.Equal(repository.NextOrderId, contracts.CreatedForOrder);
            Assert.Contains("№7", Assert.Single(messages.Infos));
            Assert.True(completed);
        }

        [Fact]
        public void Book_SavesEditedDocumentsToTheProfile()
        {
            CurrentUser customer = TestData.Customer(withDocuments: false);
            RentCarViewModel viewModel = CreateViewModel(customer);
            viewModel.PassportNumber = "MP7654321";
            viewModel.IdentificationNumber = "3120399A001PB4";
            viewModel.LicenseSeries = "9AA";
            viewModel.LicenseNumber = "654321";
            ChooseValidPeriod(viewModel);

            viewModel.Book();

            Assert.Equal("MP7654321", Assert.Single(repository.SavedRentals).PassportNumber);
            Assert.Equal("654321", customer.LicenseNumber);
        }

        [Fact]
        public void Book_WhenContractFileFails_KeepsTheBookingAndReportsTheError()
        {
            contracts.Fail = true;
            RentCarViewModel viewModel = CreateViewModel();
            ChooseValidPeriod(viewModel);

            viewModel.Book();

            Assert.Single(repository.SavedRentals);
            Assert.Contains("disk is full", Assert.Single(messages.Errors));
        }
    }
}
