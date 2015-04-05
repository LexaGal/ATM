using System;
using System.Collections.Generic;

namespace ATM.Classes
{
    public class BankAdmin
    {
        public static Dictionary<DateTime, int> ShowWithdrawnMoney()
        {
            return new Dictionary<DateTime, int>();
        }

        public static List<Client> ShowClients()
        {
            return new List<Client>();
        }

        public static Dictionary<Client, List<Operation>> ShowClientsOperations()
        {
            return new Dictionary<Client, List<Operation>>();
        }
    }
}
