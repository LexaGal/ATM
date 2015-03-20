using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ATM
{
    public class Client
    {
        
        public string UniqueKey { get; private set; }
        public string PinCode { get; private set; }
        public int CurrentSumm { get; private set; }

        public Client()
        {
            using (TextReader reader = new StreamReader(Info.ClientsPath))
            {
                var s = reader.ReadLine();

                if (String.IsNullOrEmpty(s))
                {
                    throw new FileLoadException();
                }

                var tuple = s.Split(' ');
                UniqueKey = tuple[0];
                PinCode = tuple[1];
                CurrentSumm = int.Parse(tuple[2]);
            }
        }

        public Client(string uniqueKey, string pinCode, int currentSumm)
        {
            UniqueKey = uniqueKey;
            PinCode = pinCode;
            CurrentSumm = currentSumm;
        }

        public Client(Client client)
        {
            UniqueKey = client.UniqueKey;
            PinCode = client.PinCode;
            CurrentSumm = client.CurrentSumm;
        }

        public bool EnterKeyAndPinCode(string key, string pinCode)
        {
            return BankTerminal.CheckKeyAndPinCode(key, pinCode).Item2;
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
                
            return BankTerminal.ReturnBanknotes();
        }

        public int WatchBalance()
        {
            return BankTerminal.ShowBalance();
        }
        
    }
}
