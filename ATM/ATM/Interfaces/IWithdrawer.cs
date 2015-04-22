using System;
using ATM.Classes;

namespace ATM.Interfaces
{
    interface IWithdrawer
    {
        void ConnectToBank(Bank bank);
        void InsertBanknotes(BanknotesPack banknotesPack);
        Tuple<Client, bool> ServiceNewClient(string key, string pinCode);
        Tuple<BanknotesPack, int, string> WithdrawMoney(ref int summ);
    }
}
