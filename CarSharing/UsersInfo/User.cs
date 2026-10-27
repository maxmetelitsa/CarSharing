using System.Collections.Generic;

namespace CarSharing.UsersInfo
{
    public class User
    {
        private static int lastID = 0;
        public int ID { get; }
        private string name;
        private string surname;
        private string email;
        private string password;
        private string passportNumber;
        private string identificationNumber;
        private string licenseSeries;
        private string licenseNumber;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Surname
        {
            get { return surname; }
            set { surname = value; }
        }
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
        public string PassportNumber
        {
            get { return passportNumber;}
            set { passportNumber = value; }
        }
        public string IdentificationNumber
        {
            get { return identificationNumber; }
            set { identificationNumber = value; }
        }
        public string LicenseSeries
        {
            get { return licenseSeries; }
            set { licenseSeries = value; }
        }
        public string LicenseNumber
        {
            get { return licenseNumber; }
            set { licenseNumber = value; }
        }

        public User(string name, string surname, string email, string password)
        {
            ID = ++lastID;
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
            PassportNumber = null;
            IdentificationNumber = null;
            LicenseSeries = null;
            LicenseNumber = null;
        }
    }
}
