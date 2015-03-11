using System;
using System.Collections.Generic;
using System.IO;

namespace ATM
{
    public class Bank
    {
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
            try
            {
                using (TextReader reader = new StreamReader(Info.PackPath))
                {
                    string s;
                    do
                    {
                        s = reader.ReadLine();

                        if (String.IsNullOrEmpty(s))
                        {
                            continue;
                        }

                        var pair = s.Split(' ');
                        Banknotes.Add(new Tuple<Banknote, int>(new Banknote(int.Parse(pair[0])), int.Parse(pair[1])));
                    } while (s != null);
                }
            }
            
            catch (FileNotFoundException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return new BanknotesPack(Banknotes);
        }
    }
}
