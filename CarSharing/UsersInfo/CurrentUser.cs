using CarSharing.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace CarSharing.UsersInfo
{
    public class CurrentUser
    {
        public int id;
        string name;
        string surname;
        string email;
        string password;
        string passportNumber;
        string identificationNumber;
        string licenseSeries;
        string licenseNumber;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
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
            get { return passportNumber; }
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

        public CurrentUser(int id, string name, string surname, string email, string password, string passportNumber, string identificationNumber, string licenseSeries, string licenseNumber)
        {
            ID = id;
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
            PassportNumber = passportNumber;
            IdentificationNumber = identificationNumber;
            LicenseSeries = licenseSeries;
            LicenseNumber = licenseNumber;
        }
        public CurrentUser(string name, string surname, string email, string password)
        {
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
        }

    }
}
