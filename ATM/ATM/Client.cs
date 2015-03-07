using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ATM
{
    public class Client
    {
        private const string Path = @"C:\Users\Alex\Desktop\ATM\ATM\Clients.txt";
        
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
                    if (!String.IsNullOrEmpty(s))
                    {
                        string[] pair = s.Split(' ');
                        PinCode = pair[0];
                        CurrentSumm = int.Parse(pair[1]);  
                    }

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

        public StringBuilder GetMoney(int summ)
        {
            BankTerminal.RequestedSumm = summ;

            var banknotes = new StringBuilder("\n");

            try
            {
                var result = BankTerminal.WithdrawMoney(ref summ);

                if (result.Item1 == -1)
                {
                    Console.WriteLine(result.Item2);
                    return null;
                }

                CurrentSumm = result.Item1;

                if (BankTerminal.WithdrawnMoney.BanknotesOut.Count == 0) return null;

                BankTerminal.WithdrawnMoney.BanknotesOut.OrderByDescending(tuple =>
                                                                           tuple.Item1.Weight).Where(
                                                                               elem1 => elem1.Item2 != 0).ToList().
                    ForEach(elem2 =>
                            banknotes.Append(elem2.Item1.Weight.ToString() + " * " + elem2.Item2.ToString() + " + "));


                banknotes.Remove(banknotes.ToString().Count() - 3, 3);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return banknotes;
        }

        public void SaveState()
        {
            using (TextWriter writer = new StreamWriter(Path))
            {
                writer.WriteLine(PinCode +  ' ' + CurrentSumm.ToString());
            }
        }
    }
}
