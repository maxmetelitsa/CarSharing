using CarSharing.OrdersInfo;
using CarSharing.SQL_Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarSharing.UserOrders
{
    public class UserOrderManager
    {
        public UserOrders userOrdersList;
        public UserOrderManager()
        {
            userOrdersList = new UserOrders();
        }
    }
}