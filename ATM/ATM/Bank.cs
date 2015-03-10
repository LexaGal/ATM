using System;
using System.Collections.Generic;
using System.IO;

namespace ATM
{
    public class Bank
    {
        private const string Path =
            @"C:\Users\Alex\Documents\GitHub\ATM\ATM\ATM\ATM\bin\Debug\Pack.txt";

        public int AllMoney { get; set; }
        public List<Client> Clients { get; private set; }
        public List<Operation> Operations { get; private set; }
        public List<Tuple<Banknote, int>> Banknotes { get; private set; }

        public Bank(int allMoney)
        {
            AllMoney = allMoney;
            Clients = new List<Client>();
            Operations = new List<Operation>();
            Banknotes = new List<Tuple<Banknote, int>>();
        }

        public BanknotesPack CreateBanknotesPack()
        {
            using (TextReader reader = new StreamReader(Path))
            {
                string s;
                do
                {
                    s = reader.ReadLine();
                    if (String.IsNullOrEmpty(s)) continue;
                    var pair = s.Split(' ');
                    Banknotes.Add(new Tuple<Banknote, int>(new Banknote(int.Parse(pair[0])), int.Parse(pair[1])));
                } while (s != null);
            }

            return new BanknotesPack(Banknotes);
        }
    }

    public class Operation
    {
        public Operation()
        {}
    }

    public class Banknote
    {
        public int Weight { get; private set; }
        
        public Banknote(int weight)
        {
            Weight = weight;
        }
    }

    public class BanknotesPack
    {
        public List<Tuple<Banknote, int>> Banknotes { get; private set; }
        
        public BanknotesPack(IEnumerable<Tuple<Banknote, int>> banknotes)
        {
            Banknotes = new List<Tuple<Banknote, int>>(banknotes);
        }

        public BanknotesPack(BanknotesPack banknotesPack)
        {
            Banknotes = new List<Tuple<Banknote, int>>(banknotesPack.Banknotes);
        }
    }
}
