using System;
using System.Collections.Generic;
using CarSharing.CarsInfo;
using CarSharing.Data;
using CarSharing.Domain;
using CarSharing.Mvvm;
using CarSharing.Services;
using CarSharing.UsersInfo;

namespace CarSharing.ViewModels
{
    /// <summary>Car booking form: customer documents, rental period, price and booking.</summary>
    public class RentCarViewModel : ViewModelBase
    {
        private readonly CurrentUser customer;
        private readonly IRentalRepository repository;
        private readonly IContractService contractService;
        private readonly IMessageService messages;
        private readonly Func<DateTime> now;

        private string name;
        private string surname;
        private string passportNumber;
        private string identificationNumber;
        private string licenseSeries;
        private string licenseNumber;
        private DateTime? startDate;
        private DateTime? endDate;
        private RentalQuote quote;
        private bool isEditing;

        private Dictionary<string, string> errors = new Dictionary<string, string>();

        public RentCarViewModel(
            Car car,
            CurrentUser customer,
            IRentalRepository repository,
            IContractService contractService,
            IMessageService messages,
            Func<DateTime> now = null)
        {
            Card = new CarCardViewModel(car);
            this.customer = customer ?? throw new ArgumentNullException(nameof(customer));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.contractService = contractService ?? throw new ArgumentNullException(nameof(contractService));
            this.messages = messages ?? throw new ArgumentNullException(nameof(messages));
            this.now = now ?? (() => DateTime.Now);

            // The form starts with the data from the customer's profile.
            name = customer.Name;
            surname = customer.Surname;
            passportNumber = customer.PassportNumber;
            identificationNumber = customer.IdentificationNumber;
            licenseSeries = customer.LicenseSeries;
            licenseNumber = customer.LicenseNumber;

            // A customer without saved documents has to fill them in, so the form opens for editing.
            isEditing = string.IsNullOrWhiteSpace(passportNumber) || string.IsNullOrWhiteSpace(licenseNumber);

            BookCommand = new RelayCommand(Book);
            EditCommand = new RelayCommand(() => IsEditing = true);
        }

        /// <summary>Raised after the car has been booked; the view navigates to the profile.</summary>
        public event Action BookingCompleted;

        public CarCardViewModel Card { get; }

        public RelayCommand BookCommand { get; }

        public RelayCommand EditCommand { get; }

        public bool IsEditing
        {
            get => isEditing;
            set
            {
                if (SetProperty(ref isEditing, value))
                {
                    OnPropertyChanged(nameof(IsReadOnly));
                }
            }
        }

        /// <summary>Fields are read-only until the customer presses "Изменить".</summary>
        public bool IsReadOnly => !IsEditing;

        // ---- Customer fields (email comes from the account and cannot be changed here) ----

        public string Name { get => name; set => SetProperty(ref name, value); }

        public string Surname { get => surname; set => SetProperty(ref surname, value); }

        public string Email => customer.Email;

        public string PassportNumber { get => passportNumber; set => SetProperty(ref passportNumber, value); }

        public string IdentificationNumber { get => identificationNumber; set => SetProperty(ref identificationNumber, value); }

        public string LicenseSeries { get => licenseSeries; set => SetProperty(ref licenseSeries, value); }

        public string LicenseNumber { get => licenseNumber; set => SetProperty(ref licenseNumber, value); }

        // ---- Rental period and price ----

        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                if (SetProperty(ref startDate, value))
                {
                    RecalculatePrice();
                }
            }
        }

        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                if (SetProperty(ref endDate, value))
                {
                    RecalculatePrice();
                }
            }
        }

        public string DaysText => quote != null && quote.IsValid ? quote.Days.ToString() : string.Empty;

        public string TotalText => quote != null && quote.IsValid ? quote.Total.ToString("0.##") : string.Empty;

        // ---- Validation messages shown under the fields ----

        public string NameError => GetError(RentalFormValidator.Name);
        public string SurnameError => GetError(RentalFormValidator.Surname);
        public string EmailError => GetError(RentalFormValidator.Email);
        public string PassportNumberError => GetError(RentalFormValidator.PassportNumber);
        public string IdentificationNumberError => GetError(RentalFormValidator.IdentificationNumber);
        public string LicenseSeriesError => GetError(RentalFormValidator.LicenseSeries);
        public string LicenseNumberError => GetError(RentalFormValidator.LicenseNumber);
        public string StartDateError => GetError(RentalFormValidator.StartDate);

        /// <summary>End date error, or why the selected period cannot be rented.</summary>
        public string EndDateError
        {
            get
            {
                string error = GetError(RentalFormValidator.EndDate);
                if (error.Length == 0 && quote != null && !quote.IsValid)
                {
                    return quote.Error;
                }

                return error;
            }
        }

        public void Book()
        {
            RentalForm form = CreateForm();
            errors = RentalFormValidator.Validate(form, now());
            RaiseErrorsChanged();
            if (errors.Count > 0)
            {
                IsEditing = true;
                return;
            }

            RentalQuote rentalQuote = RentalCalculator.Calculate(form.StartDate.Value, form.EndDate.Value, Card.DayPrice);
            if (!rentalQuote.IsValid)
            {
                messages.ShowError(rentalQuote.Error, "Невозможно забронировать");
                return;
            }

            if (!repository.IsAvailable(Card.Car.ID, form.StartDate.Value, form.EndDate.Value))
            {
                messages.ShowError("Автомобиль уже забронирован на выбранные даты.", "Невозможно забронировать");
                return;
            }

            int orderId = repository.SaveRental(new RentalRecord
            {
                CustomerId = customer.ID,
                CarId = Card.Car.ID,
                StartDate = form.StartDate.Value,
                EndDate = form.EndDate.Value,
                TotalPrice = rentalQuote.Total,
                PassportNumber = form.PassportNumber,
                IdentificationNumber = form.IdentificationNumber,
                LicenseSeries = form.LicenseSeries,
                LicenseNumber = form.LicenseNumber
            });

            customer.PassportNumber = form.PassportNumber;
            customer.IdentificationNumber = form.IdentificationNumber;
            customer.LicenseSeries = form.LicenseSeries;
            customer.LicenseNumber = form.LicenseNumber;

            try
            {
                contractService.CreateRentalContracts(orderId, Card.Car, form, rentalQuote);
            }
            catch (Exception ex)
            {
                messages.ShowError($"Бронь №{orderId} сохранена, но файл договора создать не удалось: {ex.Message}", "Ошибка");
                BookingCompleted?.Invoke();
                return;
            }

            messages.ShowInfo($"Автомобиль успешно забронирован. Договор №{orderId}", "Бронирование");
            BookingCompleted?.Invoke();
        }

        private RentalForm CreateForm()
        {
            return new RentalForm
            {
                Name = Name?.Trim(),
                Surname = Surname?.Trim(),
                Email = Email?.Trim(),
                PassportNumber = PassportNumber?.Trim(),
                IdentificationNumber = IdentificationNumber?.Trim(),
                LicenseSeries = LicenseSeries?.Trim(),
                LicenseNumber = LicenseNumber?.Trim(),
                StartDate = StartDate,
                EndDate = EndDate
            };
        }

        private void RecalculatePrice()
        {
            quote = StartDate.HasValue && EndDate.HasValue
                ? RentalCalculator.Calculate(StartDate.Value, EndDate.Value, Card.DayPrice)
                : null;

            OnPropertyChanged(nameof(DaysText));
            OnPropertyChanged(nameof(TotalText));
            OnPropertyChanged(nameof(EndDateError));
        }

        private string GetError(string field)
        {
            return errors.TryGetValue(field, out string error) ? error : string.Empty;
        }

        private void RaiseErrorsChanged()
        {
            OnPropertyChanged(nameof(NameError));
            OnPropertyChanged(nameof(SurnameError));
            OnPropertyChanged(nameof(EmailError));
            OnPropertyChanged(nameof(PassportNumberError));
            OnPropertyChanged(nameof(IdentificationNumberError));
            OnPropertyChanged(nameof(LicenseSeriesError));
            OnPropertyChanged(nameof(LicenseNumberError));
            OnPropertyChanged(nameof(StartDateError));
            OnPropertyChanged(nameof(EndDateError));
        }
    }
}
