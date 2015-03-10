using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ATM
{
    public class GotMoney
    {
        public List<Tuple<Banknote, int>> BanknotesOut;

        public GotMoney()
        {
            BanknotesOut = new List<Tuple<Banknote, int>>();
        }
    }

    public class BankTerminal
    {
        private const string Path =
            @"C:\Users\Alex\Documents\GitHub\ATM\ATM\ATM\ATM\bin\Debug\Pack.txt";

        private const string NoMoney =
            "Sorry, you cannot get such summ (not enough money in ATM)";

        private const string NoCombination =
            "Sorry, you cannot get such summ (ATM cannot complete your summ)";

        private const string Successed = "Successed";
        
        public static Client CurrentClient { get; private set; }
        public static BanknotesPack CurrentPack { get; private set; }

        public static int FullSumm { get; private set; }
        public static int RequestedSumm { get; set; }
        public static int NBanknotesKinds { get; private set; }

        public static GotMoney WithdrawnMoney { get; private set; }
        public static List<int> BanknotesOut;
        

        public static void ServiceNewClient(Client client)
        {
            BanknotesOut = new List<int>();

            WithdrawnMoney = new GotMoney();

            CurrentClient = new Client(client);

            WithdrawnMoney.BanknotesOut = new List<Tuple<Banknote, int>>();

            foreach (var tuple in CurrentPack.Banknotes)
            {
                WithdrawnMoney.BanknotesOut.Add(new Tuple<Banknote, int>(tuple.Item1, 0));
            }
        }

        public static void InsertBanknotes(BanknotesPack banknotesPack)
        {
            CurrentPack = new BanknotesPack(banknotesPack);
            FullSumm = 0;

            foreach (var tuple in CurrentPack.Banknotes)
            {
                FullSumm += tuple.Item1.Weight*tuple.Item2;
            }

            NBanknotesKinds = CurrentPack.Banknotes.Count;
        }

        public static bool EnteredRightPinCode(string pinCode)
        {
            return true;
        }

        public static Tuple<List<Tuple<Banknote, int>>, int, string> WithdrawMoney(ref int summ)
        {
            if (summ > FullSumm)
                return new Tuple<List<Tuple<Banknote, int>>, int, string>(null , -1, NoMoney);

            //probabilities of choice of each banknote
            var probabilities = new List<Tuple<Banknote, double>>();

            var counter = 0;
            foreach (var tuple in CurrentPack.Banknotes)
            {
                //n banknotes of this weight available in cassette(for ex. 200: 45)
                var nBanknotesAvailable = tuple.Item2;

                //max n banknotes of this weight required for this summ (for ex. 200: 950 -> 4*200 + 150 -> 4) 
                var nBanknotesRequired = (double) summ/tuple.Item1.Weight;

                //n banknotes of this weight already given out
                var givenOut = WithdrawnMoney.BanknotesOut[counter].Item2;

                //the fraction (for ex. 200: 45/4 = 11.231...) 
                double probability;

                if (nBanknotesRequired < 1)
                    probability = -1;

                else probability = nBanknotesAvailable/nBanknotesRequired/(givenOut + 1);

                probabilities.Add(new Tuple<Banknote, double>(new Banknote(tuple.Item1.Weight), probability));

                counter++;
            }

            probabilities.Sort((tuple1, tuple2) => tuple1.Item2.CompareTo(tuple2.Item2));

            if (NBanknotesKinds == 0)
                return new Tuple<List<Tuple<Banknote, int>>, int, string>(null, -1, NoMoney);
            
            //tuple with max probability
            var max = new Tuple<Banknote, double>(probabilities[NBanknotesKinds - 1].Item1,
                                                  probabilities[NBanknotesKinds - 1].Item2);

            //index of max in banknotes out
            var index = CurrentPack.Banknotes.FindIndex(tuple => tuple.Item1.Weight == max.Item1.Weight);
            
            //decrease n of banknotes of this weight in current pack
            CurrentPack.Banknotes[index] = new Tuple<Banknote, int>(CurrentPack.Banknotes[index].Item1,
                                                                    CurrentPack.Banknotes[index].Item2 - 1);

            BanknotesOut.Add(max.Item1.Weight);

            if (CurrentPack.Banknotes[index].Item2 == 0)
            {
                CurrentPack.Banknotes.Remove(CurrentPack.Banknotes[index]);
                NBanknotesKinds--;
            }

            summ -= max.Item1.Weight;

            if (summ == 0)
            {
                FullSumm -= RequestedSumm;
                WithdrawnMoney.BanknotesOut = CalculateBanknotes();
                return new Tuple<List<Tuple<Banknote, int>>, int, string>(WithdrawnMoney.BanknotesOut,
                    CurrentClient.CurrentSumm - RequestedSumm, Successed);
            }

            if (summ < CurrentPack.Banknotes.Min(tuple => tuple.Item1.Weight) && CurrentPack.Banknotes.Count != 0)
            {
                counter = 0;
                try
                {
                    do
                    {
                        summ += BanknotesOut.Last();

                        var weights = CurrentPack.Banknotes.Where(el => el.Item1.Weight == BanknotesOut.Last());

                        index = CurrentPack.Banknotes.IndexOf(weights.ToList().First());

                        CurrentPack.Banknotes[index] = new Tuple<Banknote, int>(CurrentPack.Banknotes[index].Item1,
                                                                                CurrentPack.Banknotes[index].Item2 + 1);

                        BanknotesOut.Remove(BanknotesOut.Last());

                    } while (!BanknotesChanged(summ, ref counter));

                    for (int i = BanknotesOut.Count - counter; i < BanknotesOut.Count; i++)
                    {
                        var weights = CurrentPack.Banknotes.Where(el => el.Item1.Weight == BanknotesOut[i]);

                        index = CurrentPack.Banknotes.IndexOf(weights.ToList().First());

                        CurrentPack.Banknotes[index] = new Tuple<Banknote, int>(CurrentPack.Banknotes[index].Item1,
                                                                                CurrentPack.Banknotes[index].Item2 - 1);
                    }
                    
                    FullSumm -= RequestedSumm;

                    BanknotesOut.Sort();
                    
                    WithdrawnMoney.BanknotesOut = CalculateBanknotes();
                }

                catch (Exception)
                {
                    return new Tuple<List<Tuple<Banknote, int>>, int, string>(null,
                        CurrentClient.CurrentSumm, NoCombination);
                }

                return new Tuple<List<Tuple<Banknote, int>>, int, string>(WithdrawnMoney.BanknotesOut,
                    CurrentClient.CurrentSumm - RequestedSumm, Successed);
            }

            return WithdrawMoney(ref summ);
        }

        public static bool BanknotesChanged(int residue, ref int counter)
        {
            var allBanknotes = new List<int>();
            allBanknotes.AddRange(CurrentPack.Banknotes.Where((tuple => tuple.Item1.Weight <= residue)).
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
                return false;

            var prev = residue;
            var partialSumm = 0;

            while (prev != 0)
            {
                var banknote = prev - previousSummMain[prev];
                
                prev = previousSummMain[previousSummMain[prev]];

                BanknotesOut.Add(banknote);

                partialSumm += banknote;

                if (previousSummMain[prev] == 0)
                {
                    prev = residue - partialSumm;
                }

                counter++;
            }

            return true;
        }

        public static List<Tuple<Banknote, int>> CalculateBanknotes()
        {
            var banknotes = BanknotesOut.OrderByDescending(el => el).ToList();
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


            return tuples;
        }


        public static void ShowBalance()
        {
        }

        public static bool MoneyTransfered(int summ, Operation operation)
        {
            return true;
        }

        public static void Finish()
        {
            using (TextWriter writer = new StreamWriter(Path))
            {
                foreach (var tuple in CurrentPack.Banknotes)
                {
                    writer.WriteLine(tuple.Item1.Weight.ToString() + ' ' + tuple.Item2.ToString());
                }
            }
        }
        
    }
}
