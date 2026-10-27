using System.IO;
using CarSharing.CarsInfo;
using CarSharing.CompanyInfo;
using CarSharing.Domain;

namespace CarSharing.Services
{
    public interface IContractService
    {
        /// <summary>Creates the rental contract (.docx) for the company and the customer's copy.</summary>
        void CreateRentalContracts(int orderId, Car car, RentalForm form, RentalQuote quote);
    }

    public class ContractService : IContractService
    {
        private static Company CreateCompany()
        {
            return new Company("Car House", "220036, Беларусь, Минск, ул. Карла Либкнехта 129",
                "+375 44 547 22 52", "carhouse_minsk@info.by", "Альфа-Банк",
                "BY51ALPHA301840GHB70010290000", "BY51ALPHA2873197VTB40020130000", "153001550", "790110246");
        }

        public void CreateRentalContracts(int orderId, Car car, RentalForm form, RentalQuote quote)
        {
            Directory.CreateDirectory(AppPaths.RentalsDirectory);
            Directory.CreateDirectory(AppPaths.UserRentalsDirectory);

            string[] outputPaths =
            {
                Path.Combine(AppPaths.RentalsDirectory, $"rental_order{orderId}.docx"),
                Path.Combine(AppPaths.UserRentalsDirectory, $"user_order{orderId}.docx")
            };

            Company company = CreateCompany();
            foreach (string path in outputPaths)
            {
                company.GenerateRentalDocument(path, car, form.Name, form.Surname, form.Email,
                    form.PassportNumber, form.IdentificationNumber, form.LicenseSeries, form.LicenseNumber,
                    form.StartDate.Value, form.EndDate.Value, quote.Days, quote.Total);
            }
        }
    }
}
