using System;

namespace ATM
{
    public class Actor
    {
        public static void GetAccess()
        {
            while (true)
            {
                Console.Write("\nInput UniqueKey: ");
                var key = Console.ReadLine();

                Console.Write("\nInput PIN-code: ");
                var pinCode = Console.ReadLine();

                var newClient = BankTerminal.CheckKeyAndPinCode(key, pinCode);
                if (newClient.Item2)
                {
                    BankTerminal.UpdateClientData(newClient.Item1);
                    PerformOperation();
                }

                Console.WriteLine("Wrong input data.\n");
            }
        }

        public static void PerformOperation()
        {
            BankTerminal.UpdateClientData(BankTerminal.CurrentClient);

            Console.WriteLine("\nSumm in ATM: {0}", BankTerminal.FullSumm);
            Console.WriteLine("Client's current summ: {0}", BankTerminal.CurrentClient.CurrentSumm);

            Console.Write("Input the summ to get: ");
            var money = Console.ReadLine();
            var summ = 0;

            if (!String.IsNullOrEmpty(money))
            {
                summ = int.Parse(money);
            }

            if (summ <= 0)
            {
                Console.WriteLine("Input string was not in a correct format.\n");
                PerformOperation();
            }

            var gotMoney = BankTerminal.CurrentClient.GetMoney(summ);

            Console.WriteLine(gotMoney);

            BankTerminal.FinishService();

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
            var bank = new Bank(int.MaxValue);
            BankTerminal.ConnectToBank(bank);
            BankTerminal.InsertBanknotes(bank.FullBanknotesPack);

            GetAccess();
            PerformOperation();
            
        }
    }
}
