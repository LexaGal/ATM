using System;
using System.Collections.Generic;
using System.Linq;

namespace ATM.Classes
{
    public class BanknotesPack
    {
        public List<Tuple<Banknote, int>> Banknotes { get; set; }
        
        public int Summ
        {
            get
            {
                return Banknotes.Sum(banknote => banknote.Item1.Weight*banknote.Item2);
            }
        }

        public BanknotesPack()
        {
            Banknotes = new List<Tuple<Banknote, int>>();
        }

        public BanknotesPack(List<Tuple<Banknote, int>> banknotes)
        {
            Banknotes = new List<Tuple<Banknote, int>>(banknotes);
        }

        public BanknotesPack(BanknotesPack banknotesPack)
        {
            Banknotes = new List<Tuple<Banknote, int>>(banknotesPack.Banknotes);
        }

        public void Clear()
        {
            Banknotes.Clear();
        }
    }
}