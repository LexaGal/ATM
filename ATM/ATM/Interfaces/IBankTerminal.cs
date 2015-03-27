using System;

namespace ATM.Interfaces
{
    interface IBankTerminal
    {
        void ConnectToBank(Bank bank);
        void InsertBanknotes(BanknotesPack banknotesPack);
        Tuple<Client, bool> ServiceNewClient(string key, string pinCode);
        Tuple<BanknotesPack, int, string> WithdrawMoney(ref int summ);
    }
}
