using CarSharing.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Windows;

namespace CarSharing.UsersInfo
{
    public class CurrentUserManager
    {
        private static CurrentUser currentUser;
        public SQLiteManager manager = new SQLiteManager();

        public static CurrentUser CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; }
        }
    }
}
