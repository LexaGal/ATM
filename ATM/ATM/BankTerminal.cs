using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ATM.Interfaces;

namespace ATM
{
    public class BankTerminal : IBankTerminal
    {
        public Client CurrentClient { get; private set; }
        public Bank CurrentBank { get; private set; }
        public BanknotesPack AllBanknotesPack { get; private set; }

        public int FullSumm { get; private set; }
        public int RequestedSumm { get; set; }
        public int NBanknotesKinds { get; private set; }

        public BanknotesPack ClientBanknotesPack { get; private set; }
        public List<int> Banknotes { get; set; }

        public void ConnectToBank(Bank bank)
        {
            CurrentBank = new Bank(bank);
        }

        public void InsertBanknotes(BanknotesPack banknotesPack)
        {
            AllBanknotesPack = new BanknotesPack(banknotesPack);
            FullSumm = 0;

            foreach (var tuple in AllBanknotesPack.Banknotes)
            {
                FullSumm += tuple.Item1.Weight * tuple.Item2;
            }

            NBanknotesKinds = AllBanknotesPack.Banknotes.Count;
        }

        public void UpdateClientData(Client client)
        {
            CurrentClient = client;

            Banknotes = new List<int>();

            ClientBanknotesPack = new BanknotesPack();

            foreach (var tuple in AllBanknotesPack.Banknotes)
            {
                ClientBanknotesPack.Banknotes.Add(new Tuple<Banknote, int>(tuple.Item1, 0));
            }
        }

        public Tuple<Client, bool> ServiceNewClient(string key, string pinCode)
        {
            foreach (var client in CurrentBank.Clients.Where(client =>
                client.UniqueKey == key && client.PinCode == pinCode))
            {
                client.CurrentBankTerminal = this;

                return new Tuple<Client, bool>(client, true);
            }

            return new Tuple<Client, bool>(null, false);
        }

        public Tuple<BanknotesPack, int, string> WithdrawMoney(ref int summ)
        {
            if (summ > FullSumm)
            {
                return new Tuple<BanknotesPack, int, string>(null, -1, Info.OperationErrorNotEnoughMoneyInATM);
            }

            //probabilities of choice of each banknote
            var probabilities = new List<Tuple<Banknote, double>>();

            var counter = 0;
            foreach (var tuple in AllBanknotesPack.Banknotes)
            {
                //n banknotes of this weight available in cassette(for ex. 200: 45)
                var nBanknotesAvailable = tuple.Item2;

                //max n banknotes of this weight required for this summ (for ex. 200: 950 -> 4*200 + 150 -> 4) 
                var nBanknotesRequired = (double) summ / tuple.Item1.Weight;

                //n banknotes of this weight already given out
                var givenOut = ClientBanknotesPack.Banknotes[counter].Item2;

                //the fraction (for ex. 200: 45/4 = 11.231...) 
                var probability = nBanknotesAvailable / nBanknotesRequired / (givenOut + 1);

                if (nBanknotesRequired < 1)
                {
                    probability = -1;
                }

                probabilities.Add(new Tuple<Banknote, double>(new Banknote(tuple.Item1.Weight), probability));

                counter++;
            }

            probabilities.Sort((tuple1, tuple2) => tuple1.Item2.CompareTo(tuple2.Item2));

            if (NBanknotesKinds == 0)
            {
                return new Tuple<BanknotesPack, int, string>(null, -1, Info.OperationErrorNotEnoughMoneyInATM);
            }
            
            //tuple with max probability
            var max = new Tuple<Banknote, double>(probabilities[NBanknotesKinds - 1].Item1,
                                                  probabilities[NBanknotesKinds - 1].Item2);

            //index of max in banknotes out
            var index = AllBanknotesPack.Banknotes.FindIndex(tuple => tuple.Item1.Weight == max.Item1.Weight);
            
            //decrease n of banknotes of this weight in current pack
            AllBanknotesPack.Banknotes[index] = new Tuple<Banknote, int>(AllBanknotesPack.Banknotes[index].Item1,
                                                                    AllBanknotesPack.Banknotes[index].Item2 - 1);

            Banknotes.Add(max.Item1.Weight);

            if (AllBanknotesPack.Banknotes[index].Item2 == 0)
            {
                AllBanknotesPack.Banknotes.Remove(AllBanknotesPack.Banknotes[index]);
                NBanknotesKinds--;
            }

            summ -= max.Item1.Weight;

            if (summ == 0)
            {
                FullSumm -= RequestedSumm;
                ClientBanknotesPack = CreateBanknotesPack();
                return new Tuple<BanknotesPack, int, string>(ClientBanknotesPack,
                    CurrentClient.CurrentSumm - RequestedSumm, Info.OperationSuccessed);
            }

            if (summ < AllBanknotesPack.Banknotes.Min(tuple => tuple.Item1.Weight) && AllBanknotesPack.Banknotes.Count != 0)
            {
                counter = 0;
                try
                {
                    do
                    {
                        summ += Banknotes.Last();

                        var weights = AllBanknotesPack.Banknotes.Where(el => el.Item1.Weight == Banknotes.Last());

                        index = AllBanknotesPack.Banknotes.IndexOf(weights.ToList().First());

                        AllBanknotesPack.Banknotes[index] = new Tuple<Banknote, int>(AllBanknotesPack.Banknotes[index].Item1,
                                                                                AllBanknotesPack.Banknotes[index].Item2 + 1);

                        Banknotes.Remove(Banknotes.Last());

                    } while (!GetBanknotesCombination(summ, ref counter));

                    for (int i = Banknotes.Count - counter; i < Banknotes.Count; i++)
                    {
                        var weights = AllBanknotesPack.Banknotes.Where(el => el.Item1.Weight == Banknotes[i]);

                        index = AllBanknotesPack.Banknotes.IndexOf(weights.ToList().First());

                        AllBanknotesPack.Banknotes[index] = new Tuple<Banknote, int>(AllBanknotesPack.Banknotes[index].Item1,
                                                                                AllBanknotesPack.Banknotes[index].Item2 - 1);

                        if (AllBanknotesPack.Banknotes[index].Item2 == 0)
                        {
                            AllBanknotesPack.Banknotes.Remove(AllBanknotesPack.Banknotes[index]);
                            NBanknotesKinds--;
                        }

                    }
                    
                    FullSumm -= RequestedSumm;

                    Banknotes.Sort();
                    
                    ClientBanknotesPack = CreateBanknotesPack();
                }

                catch (InvalidOperationException)
                {
                    return new Tuple<BanknotesPack, int, string>(null,
                        CurrentClient.CurrentSumm, Info.OperationErrorNoBanknotesCombination);
                }

                return new Tuple<BanknotesPack, int, string>(ClientBanknotesPack,
                    CurrentClient.CurrentSumm - RequestedSumm, Info.OperationSuccessed);
            }

            return WithdrawMoney(ref summ);
        }

        public string ReturnBanknotes()
        {
            var banknotes = new StringBuilder("\n");
            
            ClientBanknotesPack.Banknotes.OrderByDescending(tuple =>
                tuple.Item1.Weight).Where(elem1 => elem1.Item2 != 0).ToList().
                    ForEach(elem2 => banknotes.Append(String.Format("{0} * {1}\n", elem2.Item1.Weight.ToString(),
                        elem2.Item2.ToString())));
                        
            return banknotes.ToString();
        }

        public bool GetBanknotesCombination(int residue, ref int counter)
        {
            var allBanknotes = new List<int>();
            allBanknotes.AddRange(AllBanknotesPack.Banknotes.Where((tuple => tuple.Item1.Weight <= residue)).
                Select(b => b.Item1.Weight));
            
            var nBanknotes = allBanknotes.Count;
            var maxSumm = residue + 1;
            
            var combinationExists = new List<int>(maxSumm);
            var numberOfBanknotes = new List<int>(maxSumm);
            var previousSummMain = new List<int>(maxSumm);
            var previousSummAdditional = new List<int>(maxSumm);

            for (int i = 0; i < maxSumm; i++)
            {
                combinationExists.Add(0);
                numberOfBanknotes.Add(254);
                previousSummAdditional.Add(0);
                previousSummMain.Add(0);
            }

            combinationExists[0] = 1;
            numberOfBanknotes[0] = 0;

            for (int i = 0; i < nBanknotes; i++)
            {
                for (int j = allBanknotes[i]; j < maxSumm; j++)
                {
                    if (combinationExists[j - allBanknotes[i]] != 0 && numberOfBanknotes[j] > numberOfBanknotes[j - allBanknotes[i]] + 1)
                    {
                        numberOfBanknotes[j] = numberOfBanknotes[j - allBanknotes[i]] + 1;
                        previousSummAdditional[j] = j - allBanknotes[i];
                        combinationExists[j] = 1;
                    }
                    else
                    {
                        previousSummAdditional[i] = previousSummMain[j];
                    }
                }

                for (int j = allBanknotes[i]; j < maxSumm; j++)
                {
                    previousSummMain[j] = previousSummAdditional[j];
                }
            }

            if (combinationExists[residue] == 0)
            {
                return false;
            }

            var prev = residue;
            var partialSumm = 0;

            while (prev != 0)
            {
                var banknote = prev - previousSummMain[prev];
                
                prev = previousSummMain[previousSummMain[prev]];

                Banknotes.Add(banknote);

                partialSumm += banknote;

                if (previousSummMain[prev] == 0)
                {
                    prev = residue - partialSumm;
                }

                counter++;
            }

            return true;
        }

        public BanknotesPack CreateBanknotesPack()
        {
            var banknotes = Banknotes.OrderByDescending(el => el).ToList();
            var tuples = new List<Tuple<Banknote, int>>();

            var elem = banknotes[0];
            var n = 0;

            for (int i = 0; i < banknotes.Count; i++)
            {
                if (banknotes[i] == elem)
                {
                    n++;
                }
                else
                {
                    tuples.Add(new Tuple<Banknote, int>(new Banknote(banknotes[i - 1]), n));
                    elem = banknotes[i];
                    n = 1;
                }

                if (i == banknotes.Count - 1)
                {
                    tuples.Add(new Tuple<Banknote, int>(new Banknote(banknotes[i]), n));
                }
            }

            return new BanknotesPack(tuples);
        }

        public int ShowBalance()
        {
            return CurrentClient.CurrentSumm;
        }

        public bool TransferMoney(int summ, Operation operation)
        {
            return true;
        }

        public void FinishService()  
        {
            try
            {
                using (TextWriter writer = new StreamWriter(Info.BanknotesPackPath))
                {
                    foreach (var tuple in AllBanknotesPack.Banknotes)
                    {
                        writer.WriteLine("{0} {1}", tuple.Item1.Weight, tuple.Item2);
                    }
                }

                using (TextWriter writer = new StreamWriter(Info.ClientsFilePath))
                {
                    foreach (var client in CurrentBank.Clients)
                    {
                        writer.WriteLine("{0} {1} {2}", client.UniqueKey, client.PinCode, client.CurrentSumm);
                    }
                }
            }

            catch (FileNotFoundException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

    }
}
