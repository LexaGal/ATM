using System;
using System.IO;
using ATM.Interfaces;

namespace ATM.Classes
{
    public class Client: IGetter
    {
        public BankTerminal CurrentBankTerminal { get; set; }
        public string UniqueKey { get; private set; }
        public string PinCode { get; private set; }
        public int CurrentSumm { get; private set; }
        public BanknotesReturner BanknotesReturner { get; set; }  

        public Client()
        {
            using (TextReader reader = new StreamReader(Info.ClientsFilePath))
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

        public bool ConnectToTerminal(string key, string pinCode)
        {
            return CurrentBankTerminal.ServiceNewClient(key, pinCode).Item2;
        }

        public BanknotesPack GetMoney(int summ)
        {
            if (summ > CurrentSumm)
            {
                CurrentBankTerminal.Log.Error(string.Format("Client [Key: {0} Pin: {1}] sum: {2} [Error: {3}]",
                   UniqueKey, PinCode, summ, Info.OperationErrorNotEnoughMoneyClient));

                BanknotesReturner = new BanknotesReturner(null, Info.OperationErrorNotEnoughMoneyClient);
                
                return null;
            }

            CurrentBankTerminal.RequestedSumm = summ;

            var result = CurrentBankTerminal.WithdrawMoney(ref summ);

            if (result.Item1 == null)
            {
                BanknotesReturner = new BanknotesReturner(null, result.Item3);
                
                return null;
            }

            CurrentSumm = result.Item2;

            BanknotesReturner = new BanknotesReturner(result.Item1);

            return result.Item1;
        }

        public int WatchBalance()
        {
            return CurrentBankTerminal.ShowBalance();
        }

    }

}