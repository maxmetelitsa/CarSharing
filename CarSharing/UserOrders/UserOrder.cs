using CarSharing.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarSharing.UserOrders
{
    public class UserOrder
    {
        private static int lastID = 0;
        public int ID { get; set; }
        private int orderID;
        private int userID;
        private int rentalContractNumber;
        public int OrderID
        {
            get { return orderID; }
            set { orderID = value; }
        }
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }
        public int RentalContractNumber
        {
            get { return rentalContractNumber; }
            set { rentalContractNumber = value; }
        }
        public UserOrder(int orderID, int userID, int rentalContractNumber)
        {
            ID = ++lastID;
            OrderID = orderID;
            UserID = userID;
            RentalContractNumber = rentalContractNumber;
        }
    }
}