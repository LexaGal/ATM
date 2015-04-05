namespace ATM.Interfaces
{
    interface IClient
    {
        bool ConnectToTerminal(string key, string pinCode);
        string GetMoney(int summ);
    }
}
