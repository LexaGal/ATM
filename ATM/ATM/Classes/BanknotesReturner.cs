using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ATM.Classes
{
    public class BanknotesReturner
    {
        public BanknotesPack Pack { get; set; }

        public string Error { get; set; }

        public BanknotesReturner(BanknotesPack pack, string error = null)
        {
            Pack = pack;
            Error = error;
        }

        public string ReturnBanknotes()
        {
            if (Pack == null)
            {
                return Error + "\n";

            }

            var banknotesString = new StringBuilder("\n");

            Pack.Banknotes.OrderByDescending(tuple =>
                tuple.Item1.Weight).Where(elem1 => elem1.Item2 != 0).ToList().
                    ForEach(elem2 => banknotesString.Append(String.Format("{0} * {1}\n", elem2.Item1.Weight.ToString(),
                        elem2.Item2.ToString())));

            return banknotesString.ToString();
        }
    }
}
