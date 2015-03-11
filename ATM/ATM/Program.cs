using System;

namespace ATM
{
    public class Actor
    {
        private static void Main()
        {
            var bank = new Bank(int.MaxValue);

            var pack = bank.CreateBanknotesPack();
            BankTerminal.InsertBanknotes(pack);

            while (true)
            {
                try
                {
                    var client = new Client();
                    BankTerminal.ServiceNewClient(client);

                    //-------------------------------

                    Console.WriteLine("\nSumm in ATM: {0}", BankTerminal.FullSumm);
                    Console.WriteLine("Client's current summ: {0}", client.CurrentSumm);

                    Console.Write("Input the summ to get: ");
                    var money = Console.ReadLine();
                    var summ = 0;

                    if (!String.IsNullOrEmpty(money))
                    {
                        summ = int.Parse(money);
                    }

                    if (summ <= 0)
                    {
                        Console.WriteLine("Input string was not in a correct format.");
                        continue;
                    }

                    //-------------------------------

                    var gotMoney = client.GetMoney(summ);

                    Console.WriteLine(gotMoney);
                    
                    client.SaveState();
                    BankTerminal.Finish();
                }

                catch (FormatException exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }
    }
}
