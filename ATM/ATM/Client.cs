using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ATM
{
    public class Client
    {
        
        public string PinCode { get; private set; }
        public int CurrentSumm { get; private set; }

        public Client()
        {
            using (TextReader reader = new StreamReader(Info.ClientsPath))
            {
                string s;
                do
                {
                    s = reader.ReadLine();

                    if (String.IsNullOrEmpty(s))
                    {
                        continue;
                    }

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
            if (summ > CurrentSumm)
            {
                return Info.ErrorNoMoneyClient;
            }

            BankTerminal.RequestedSumm = summ;

            var result = BankTerminal.WithdrawMoney(ref summ);

            if (result.Item1 == null)
            {
                return result.Item3;
            }

            CurrentSumm = result.Item2;
                
            return ReturnBanknotes();
        }

        public string ReturnBanknotes()
        {
            var banknotes = new StringBuilder("\n");

            try
            {
                BankTerminal.WithdrawnMoney.Banknotes.OrderByDescending(tuple =>
                   tuple.Item1.Weight).Where(elem1 => elem1.Item2 != 0).ToList().
                        ForEach(elem2 => banknotes.Append(String.Format("{0} * {1}\n", elem2.Item1.Weight.ToString(),
                            elem2.Item2.ToString())));

            }

            catch(ArgumentNullException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return banknotes.ToString();
        }

        public void SaveState()
        {
            try
            {
                using (TextWriter writer = new StreamWriter(Info.ClientsPath))
                {
                    writer.WriteLine("{0} {1}", PinCode, CurrentSumm);
                }
            }

            catch (FileNotFoundException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
