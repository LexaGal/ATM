using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankTerminal
{
    public class Bank
    {
        public int AllMoney { get; set; }
        public List<Client> Clients{ get; set; }
        public List<Operation> Operations{ get; set; }
        public List<Tuple<Banknote, int>> Banknotes{ get; set; } 

        public Bank(int allmoney)
        {
            AllMoney = allmoney;
            Clients = new List<Client>();
            Operations = new List<Operation>();
            Banknotes = new List<Tuple<Banknote, int>>();
        }

        public void CreateBanknote(int weight, int number)
        {
            Banknotes.Add(new Tuple<Banknote, int>(new Banknote(weight), number));
        }

        public BanknotesPack CreateBanknotesPack()
        {
            return new BanknotesPack(Banknotes);
        }
    }
    
    public class BankAdmin
    {
        public static Dictionary<DateTime, int> ShowWithdrawnMoney()
        {
            return new Dictionary<DateTime, int>();
        }

        public static List<Client> ShowClients()
        {
            return new List<Client>();
        }

        public static Dictionary<Client, List<Operation>> ShowClientsOperations()
        {
            return new Dictionary<Client, List<Operation>>();
        }
    }
    
    public class Operation
    { }
   
    public class Banknote
    {
        public int Weight { get; set; }
        public Banknote(int weight)
        {
            Weight = weight;
        }
    }

    public class BanknotesPack
    {
        public List<Tuple<Banknote, int>> Banknotes { get; set; } 
        public BanknotesPack(List<Tuple<Banknote, int>> banknotes)
        {
            Banknotes = new List<Tuple<Banknote, int>>(banknotes);
        }

        public BanknotesPack(BanknotesPack banknotesPack)
        {
            Banknotes = new List<Tuple<Banknote, int>>(banknotesPack.Banknotes);
        }
    }
   
    public class BankTerminal
    {
        public static Client CurrentClient { get; set; }
        public static BanknotesPack CurrentPack { get; set; }
        public static int FullSumm { get; set; }
        public static List<Tuple<Banknote, int>> BanknotesOut;
        public static int NBanknotesKinds;
        
        public static void ServiceNewClient(Client client)
        {
            CurrentClient = new Client(client);
            BanknotesOut = new List<Tuple<Banknote, int>>();
            foreach(Tuple<Banknote, int> tuple in CurrentPack.Banknotes)
            {
                BanknotesOut.Add(new Tuple<Banknote, int>(tuple.Item1, 0));
            }
        }

        public static void InsertBanknotes(BanknotesPack banknotesPack)
        {
            CurrentPack = new BanknotesPack(banknotesPack);
            FullSumm = 0;
            foreach(Tuple<Banknote, int> tuple in CurrentPack.Banknotes)
            {
                FullSumm += tuple.Item1.Weight * tuple.Item2;
            }
            NBanknotesKinds = CurrentPack.Banknotes.Count;
        }

        public static bool EnteredRightPinCode(string pin)
        {
            return true;
        }

        public static Tuple<int, string> MoneyWithdrawn(ref int summ)
        {
            if (summ > FullSumm)
                return new Tuple<int, string>(-1, "Sorry, there is no such money");
            
            //probabilities of choice of each banknote
            List<Tuple<Banknote, double>> probabilities = new List<Tuple<Banknote, double>>();

            int counter = 0;
            foreach(Tuple<Banknote, int> tuple in CurrentPack.Banknotes)
            {
                //n banknotes of this weight available in cassette(for ex. 200: 45)
                double nBanknotesAvailable = tuple.Item2;
                
                //max n banknotes of this weight required for this summ (for ex. 200: 950 -> 4*200 + 150 -> 4) 
                double nBanknotesRequired = (double)summ / tuple.Item1.Weight;

                //n banknotes of this weight already given out
                int givenOut = BanknotesOut[counter].Item2;

                //the fraction (for ex. 200: 45/4 = 11.231...) 
                double probability;

                if (nBanknotesRequired < 1)
                    probability = -1;

                else probability = nBanknotesAvailable/nBanknotesRequired/(givenOut + 1);
                
                probabilities.Add(new Tuple<Banknote, double>(new Banknote(tuple.Item1.Weight), probability));

                counter++;
            }

            probabilities.Sort((tuple1, tuple2) => tuple1.Item2.CompareTo(tuple2.Item2));

            //tuple with max probability
            Tuple<Banknote, double> max = new Tuple<Banknote, double>(probabilities[NBanknotesKinds - 1].Item1,
                                                                      probabilities[NBanknotesKinds - 1].Item2);
            //index of max in banknotes out
            int index = BanknotesOut.FindIndex(tuple => tuple.Item1.Weight == max.Item1.Weight);

            //increase n of banknotes out of this weight
            BanknotesOut[index] = new Tuple<Banknote, int>(BanknotesOut[index].Item1,
                                                                BanknotesOut[index].Item2 + 1);
            //decrease n of banknotes of this weight in current pack
            CurrentPack.Banknotes[index] = new Tuple<Banknote, int>(CurrentPack.Banknotes[index].Item1, CurrentPack.Banknotes[index].Item2 - 1);

            summ -= max.Item1.Weight;

            if (summ == 0) 
                return new Tuple<int, string>(CurrentClient.CurrentSumm - summ, "Successed");
            
            if (summ < CurrentPack.Banknotes.Min(tuple => tuple.Item1.Weight))
                return new Tuple<int, string>(-1, "Cannot give you such summ");

            return MoneyWithdrawn(ref summ);
        }

        public static void ShowBalance()
        { }

        public static bool MoneyTransfered(int summ, Operation operation)
        {
            return true;
        }
    }

    public class Client
    {
        public string PinCode { get; set; }
        public int CurrentSumm { get; set; }

        public Client(string pincode, int currentsumm)
        {
            PinCode = pincode;
            CurrentSumm = currentsumm;
        }

        public Client(Client client)
        {
            PinCode = client.PinCode;
            CurrentSumm = client.CurrentSumm;
        }
        
    }
    
    

    

}
