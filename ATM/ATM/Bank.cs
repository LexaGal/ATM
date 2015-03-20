using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Linq;

namespace ATM
{
    public class Bank
    {
        public int AllMoney { get; private set; }
        public List<Client> Clients { get; private set; }
        public List<Operation> Operations { get; private set; }
        public BanknotesPack FullBanknotesPack { get; private set; }

        public Bank(int allMoney)
        {
            AllMoney = allMoney;
            Clients = GetCreatedClients();
            Operations = new List<Operation>();
            FullBanknotesPack = CreateBanknotesPack();
        }

        public Bank(Bank bank)
        {
            AllMoney = bank.AllMoney;
            Clients = bank.Clients;
            Operations = bank.Operations;
            FullBanknotesPack = bank.FullBanknotesPack;
        }

        public BanknotesPack CreateBanknotesPack()
        {
            var banknotes = new List<Tuple<Banknote, int>>();
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
                        banknotes.Add(new Tuple<Banknote, int>(new Banknote(int.Parse(pair[0])), int.Parse(pair[1])));
                    } while (s != null);
                }
            }
            
            catch (FileNotFoundException exception)
            {
                Console.WriteLine(exception.Message);
            }

            return new BanknotesPack(banknotes);
        }

        public List<Client> CreateRandomClients(int nClients)
        {
            var pin = 999;
            const int summ = 1000000;

            for (int i = 0; i < nClients; i++)
            {
                pin ++;

                var result = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                Clients.Add(new Client(result, pin.ToString(), summ));
            }

            using (TextWriter writer = new StreamWriter(Info.ClientsPath, true))
            {
                foreach (var client in Clients)
                {
                    writer.WriteLine("{0} {1} {2}", client.UniqueKey, client.PinCode, client.CurrentSumm);
                }
            }

            return Clients;
        }

        public List<Client> GetCreatedClients()
        {
            Clients = new List<Client>();
            using (TextReader reader = new StreamReader(Info.ClientsPath))
            {
                string s;
                do
                {
                    s = reader.ReadLine();

                    if (String.IsNullOrEmpty(s))
                    {
                        continue;
                    }

                    var tuple = s.Split(' ');
                    var uniqueKey = tuple[0];
                    var pinCode = tuple[1];
                    var currentSumm = int.Parse(tuple[2]);
                    var client = new Client(uniqueKey, pinCode, currentSumm);

                    Clients.Add(client);

                } while (s != null);

            }

            return Clients;
        }
    }
}
