using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATM.Interfaces
{
    interface IClient
    {
        bool ConnectToTerminal(string key, string pinCode);
        string GetMoney(int summ);
    }
}
