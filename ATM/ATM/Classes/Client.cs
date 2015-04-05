using System;
using System.IO;
using ATM.Interfaces;

namespace ATM.Classes
{
    public class Client : IClient
    {
        public BankTerminal CurrentBankTerminal { get; set; }
        public string UniqueKey { get; private set; }
        public string PinCode { get; private set; }
        public int CurrentSumm { get; private set; }

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

        public string GetMoney(int summ)
        {
            if (summ > CurrentSumm)
            {
                return Info.OperationErrorNotEnoughMoneyClient;
            }

            CurrentBankTerminal.RequestedSumm = summ;

            var result = CurrentBankTerminal.WithdrawMoney(ref summ);

            if (result.Item1 == null)
            {
                return result.Item3;
            }

            CurrentSumm = result.Item2;

            return CurrentBankTerminal.ReturnBanknotes();
        }

        public int WatchBalance()
        {
            return CurrentBankTerminal.ShowBalance();
        }

    }

}