using System;

namespace ATM
{
    public class Actor
    {
        private static Bank _bank = new Bank(int.MaxValue);
        private static BankTerminal _bankTerminal = new BankTerminal();
            
        public static void GetAccess()
        {
            while (true)
            {
                Console.Write("\nInput UniqueKey: ");
                var key = Console.ReadLine();

                Console.Write("\nInput PIN-code: ");
                var pinCode = Console.ReadLine();

                var newClient = _bankTerminal.ServiceNewClient(key, pinCode);
                if (newClient.Item2)
                {
                    _bankTerminal.UpdateClientData(newClient.Item1);
                    PerformOperation();
                }

                Console.WriteLine("Wrong input data.\n");
            }
        }

        public static void PerformOperation()
        {
            _bankTerminal.UpdateClientData(_bankTerminal.CurrentClient);

            Console.WriteLine("\nSumm in ATM: {0}", _bankTerminal.FullSumm);
            Console.WriteLine("Client's current summ: {0}", _bankTerminal.CurrentClient.CurrentSumm);

            Console.Write("Input the summ to get: ");
            var money = Console.ReadLine();
            var summ = 0;

            if (!String.IsNullOrEmpty(money))
            {
                try
                {
                    summ = int.Parse(money);
                }
                catch (FormatException exception)
                {
                    Console.WriteLine(exception.Message);
                    PerformOperation();
                }
            }

            if (summ <= 0)
            {
                Console.WriteLine("Input string was not in a correct format.\n");
                PerformOperation();
            }

            var gotMoney = _bankTerminal.CurrentClient.GetMoney(summ);

            Console.WriteLine(gotMoney);

            _bankTerminal.FinishService();

            Console.WriteLine("Would you like to perform more operations? [yes / no]");
            var response = Console.ReadLine();

            if (response != "yes")
            {
                GetAccess();
            }

            PerformOperation();
        }

        private static void Main()
        {
            _bankTerminal.ConnectToBank(_bank);
            _bankTerminal.InsertBanknotes(_bank.FullBanknotesPack);

            GetAccess();
            PerformOperation();
            
        }
    }
}
