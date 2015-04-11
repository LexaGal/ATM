using System;
using ATM.Classes;
using log4net;

namespace ATM
{
    
    public class Actor
    {
        private static Bank _bank;
        private static BankTerminal _bankTerminal;

        static Actor()
        {
            try
            {
                _bank = new Bank(int.MaxValue);
                _bankTerminal = new BankTerminal();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            
        }
    
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

            Console.Write("Would you like to perform more operations?\nAnswer[yes / no]: ");
            var response = Console.ReadLine();

            if (response != "yes")
            {
                GetAccess();
            }

            PerformOperation();
        }


        [STAThread]
        private static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();

            _bankTerminal = new BankTerminal();
            
            _bankTerminal.ConnectToBank(_bank);
            _bankTerminal.InsertBanknotes(_bank.FullBanknotesPack);

            
            GetAccess();
            PerformOperation();
            
        }

    }
}
