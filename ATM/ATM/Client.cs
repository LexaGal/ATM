using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ATM
{
    public class Client
    {
        private const string Path = @"C:\Users\Alex\Documents\GitHub\ATM\ATM\ATM\ATM\bin\Debug\Clients.txt";
        
        public string PinCode { get; private set; }
        public int CurrentSumm { get; private set; }

        public Client()
        {
            using (TextReader reader = new StreamReader(Path))
            {
                string s;
                do
                {
                    s = reader.ReadLine();
                    if (String.IsNullOrEmpty(s)) continue;
                    var pair = s.Split(' ');
                    PinCode = pair[0];
                    CurrentSumm = int.Parse(pair[1]);
                } while (s != null);
               
            }
        }

        public Client(string pinCode, int currentSumm)
        {
            PinCode = pinCode;
            CurrentSumm = currentSumm;
        }

        public Client(Client client)
        {
            PinCode = client.PinCode;
            CurrentSumm = client.CurrentSumm;
        }

        public string GetMoney(int summ)
        {
            BankTerminal.RequestedSumm = summ;

            try
            {
                var result = BankTerminal.WithdrawMoney(ref summ);

                if (result.Item1 == null)
                {
                    Console.WriteLine(result.Item3);
                    return null;
                }

                CurrentSumm = result.Item2;
                
            }
            
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                Console.ReadKey();
                return null;
            }

            return ReturnBanknotes();
        }

        public string ReturnBanknotes()
        {
            var banknotes = new StringBuilder("\n");

            try
            {
                BankTerminal.WithdrawnMoney.BanknotesOut.OrderByDescending(tuple =>
                                                                           tuple.Item1.Weight).Where(
                                                                               elem1 => elem1.Item2 != 0).ToList().
                    ForEach(elem2 => banknotes.Append(elem2.Item1.Weight.ToString()
                                                      + " * " + elem2.Item2.ToString() + " + "));

                banknotes.Remove(banknotes.ToString().Count() - 3, 3);
            }

            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return banknotes.ToString();
        }

        public void SaveState()
        {
            using (TextWriter writer = new StreamWriter(Path))
            {
                writer.WriteLine(PinCode +  ' ' + CurrentSumm);
            }
        }
    }
}
