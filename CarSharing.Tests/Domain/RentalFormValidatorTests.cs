using System;
using System.Collections.Generic;
using CarSharing.Domain;
using Xunit;

namespace CarSharing.Tests.Domain
{
    public class RentalFormValidatorTests
    {
        private static readonly DateTime Today = new DateTime(2026, 5, 1);

        private static RentalForm ValidForm()
        {
            return new RentalForm
            {
                Name = "Иван",
                Surname = "Петров",
                Email = "ivan.petrov@example.com",
                PassportNumber = "MP1234567",
                IdentificationNumber = "3010190A001PB1",
                LicenseSeries = "9AB",
                LicenseNumber = "123456",
                StartDate = Today,
                EndDate = Today.AddDays(3)
            };
        }

        [Fact]
        public void Validate_ReturnsNoErrors_ForValidForm()
        {
            Assert.Empty(RentalFormValidator.Validate(ValidForm(), Today));
        }

        [Fact]
        public void Validate_ReportsEveryEmptyField()
        {
            Dictionary<string, string> errors = RentalFormValidator.Validate(new RentalForm(), Today);

            Assert.Equal(9, errors.Count);
            Assert.Equal("Поле должно быть заполнено", errors[RentalFormValidator.LicenseNumber]);
        }

        [Fact]
        public void Validate_RequiresDriverLicence()
        {
            RentalForm form = ValidForm();
            form.LicenseSeries = " ";
            form.LicenseNumber = "";

            Dictionary<string, string> errors = RentalFormValidator.Validate(form, Today);

            Assert.True(errors.ContainsKey(RentalFormValidator.LicenseSeries));
            Assert.True(errors.ContainsKey(RentalFormValidator.LicenseNumber));
        }

        [Theory]
        [InlineData("mp1234567")]
        [InlineData("1234567")]
        [InlineData("MP12")]
        public void Validate_ReportsInvalidPassportNumber(string passport)
        {
            RentalForm form = ValidForm();
            form.PassportNumber = passport;

            Assert.True(RentalFormValidator.Validate(form, Today).ContainsKey(RentalFormValidator.PassportNumber));
        }

        [Fact]
        public void Validate_ReportsStartDateInThePast()
        {
            RentalForm form = ValidForm();
            form.StartDate = Today.AddDays(-1);

            Assert.True(RentalFormValidator.Validate(form, Today).ContainsKey(RentalFormValidator.StartDate));
        }

        [Fact]
        public void Validate_ReportsEndDateNotAfterStartDate()
        {
            RentalForm form = ValidForm();
            form.EndDate = form.StartDate;

            Assert.True(RentalFormValidator.Validate(form, Today).ContainsKey(RentalFormValidator.EndDate));
        }

        [Theory]
        [InlineData("3010190A001PB1", null)]
        [InlineData("", "Поле должно быть заполнено")]
        [InlineData("3010190A001", "Укажите полный номер")]
        [InlineData("3010190a001pb1", "Некорректный формат номера")]
        public void ValidateIdentificationNumber_ChecksLengthAndCapitalLetters(string value, string expectedError)
        {
            Assert.Equal(expectedError, RentalFormValidator.ValidateIdentificationNumber(value));
        }

        [Theory]
        [InlineData("user@example.com", true)]
        [InlineData("user@", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void EmailValidator_RecognisesValidAddresses(string email, bool expected)
        {
            Assert.Equal(expected, EmailValidator.IsValid(email));
        }
    }
}
