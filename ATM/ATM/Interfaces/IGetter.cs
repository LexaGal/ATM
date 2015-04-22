using ATM.Classes;

namespace ATM.Interfaces
{
    interface IGetter
    {
        bool ConnectToTerminal(string key, string pinCode);
        BanknotesPack GetMoney(int summ);
    }
}
