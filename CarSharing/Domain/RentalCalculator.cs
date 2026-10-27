using System;

namespace CarSharing.Domain
{
    /// <summary>Result of a rental price calculation.</summary>
    public class RentalQuote
    {
        private RentalQuote(int days, double total, string error)
        {
            Days = days;
            Total = total;
            Error = error;
        }

        public int Days { get; }

        public double Total { get; }

        /// <summary>Why the period cannot be rented, or null if it can.</summary>
        public string Error { get; }

        public bool IsValid => Error == null;

        public static RentalQuote Valid(int days, double total) => new RentalQuote(days, total, null);

        public static RentalQuote Invalid(string error) => new RentalQuote(0, 0, error);
    }

    /// <summary>Business rules for renting a car: allowed period and price.</summary>
    public static class RentalCalculator
    {
        public const int MinRentalDays = 1;
        public const int MaxRentalDays = 30;

        public static int CountDays(DateTime start, DateTime end)
        {
            return (end.Date - start.Date).Days;
        }

        public static RentalQuote Calculate(DateTime start, DateTime end, double dayPrice)
        {
            if (dayPrice < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(dayPrice), "Цена не может быть отрицательной.");
            }

            int days = CountDays(start, end);

            if (days < MinRentalDays)
            {
                return RentalQuote.Invalid($"Минимальное количество дней для аренды - {MinRentalDays}");
            }

            if (days > MaxRentalDays)
            {
                return RentalQuote.Invalid($"Количество дней аренды не должно превышать {MaxRentalDays}");
            }

            return RentalQuote.Valid(days, days * dayPrice);
        }
    }
}
