using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ATM
{
    public class BankTerminal
    {
        public static Client CurrentClient { get; private set; }
        public static BanknotesPack CurrentPack { get; private set; }

        public static int FullSumm { get; private set; }
        public static int RequestedSumm { get; set; }
        public static int NBanknotesKinds { get; private set; }

        public static BanknotesPack WithdrawnMoney { get; private set; }
        public static List<int> Banknotes { get; set; }
        

        public static void ServiceNewClient(Client client)
        {
            Banknotes = new List<int>();

            WithdrawnMoney = new BanknotesPack();

            CurrentClient = new Client(client);

            WithdrawnMoney.Banknotes = new List<Tuple<Banknote, int>>();

            foreach (var tuple in CurrentPack.Banknotes)
            {
                WithdrawnMoney.Banknotes.Add(new Tuple<Banknote, int>(tuple.Item1, 0));
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
            {
                return new Tuple<List<Tuple<Banknote, int>>, int, string>(null, -1, Info.ErrorNoMoneyATM);
            }

            //probabilities of choice of each banknote
            var probabilities = new List<Tuple<Banknote, double>>();

            var counter = 0;
            foreach (var tuple in CurrentPack.Banknotes)
            {
                //n banknotes of this weight available in cassette(for ex. 200: 45)
                var nBanknotesAvailable = tuple.Item2;

                //max n banknotes of this weight required for this summ (for ex. 200: 950 -> 4*200 + 150 -> 4) 
                var nBanknotesRequired = (double) summ / tuple.Item1.Weight;

                //n banknotes of this weight already given out
                var givenOut = WithdrawnMoney.Banknotes[counter].Item2;

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
                return new Tuple<List<Tuple<Banknote, int>>, int, string>(null, -1, Info.ErrorNoMoneyATM);
            }
            
            //tuple with max probability
            var max = new Tuple<Banknote, double>(probabilities[NBanknotesKinds - 1].Item1,
                                                  probabilities[NBanknotesKinds - 1].Item2);

            //index of max in banknotes out
            var index = CurrentPack.Banknotes.FindIndex(tuple => tuple.Item1.Weight == max.Item1.Weight);
            
            //decrease n of banknotes of this weight in current pack
            CurrentPack.Banknotes[index] = new Tuple<Banknote, int>(CurrentPack.Banknotes[index].Item1,
                                                                    CurrentPack.Banknotes[index].Item2 - 1);

            Banknotes.Add(max.Item1.Weight);

            if (CurrentPack.Banknotes[index].Item2 == 0)
            {
                CurrentPack.Banknotes.Remove(CurrentPack.Banknotes[index]);
                NBanknotesKinds--;
            }

            summ -= max.Item1.Weight;

            if (summ == 0)
            {
                FullSumm -= RequestedSumm;
                WithdrawnMoney.Banknotes = CalculateBanknotes();
                return new Tuple<List<Tuple<Banknote, int>>, int, string>(WithdrawnMoney.Banknotes,
                    CurrentClient.CurrentSumm - RequestedSumm, Info.Successed);
            }

            if (summ < CurrentPack.Banknotes.Min(tuple => tuple.Item1.Weight) && CurrentPack.Banknotes.Count != 0)
            {
                counter = 0;
                try
                {
                    do
                    {
                        summ += Banknotes.Last();

                        var weights = CurrentPack.Banknotes.Where(el => el.Item1.Weight == Banknotes.Last());

                        index = CurrentPack.Banknotes.IndexOf(weights.ToList().First());

                        CurrentPack.Banknotes[index] = new Tuple<Banknote, int>(CurrentPack.Banknotes[index].Item1,
                                                                                CurrentPack.Banknotes[index].Item2 + 1);

                        Banknotes.Remove(Banknotes.Last());

                    } while (!GotRequiredCombination(summ, ref counter));

                    for (int i = Banknotes.Count - counter; i < Banknotes.Count; i++)
                    {
                        var weights = CurrentPack.Banknotes.Where(el => el.Item1.Weight == Banknotes[i]);

                        index = CurrentPack.Banknotes.IndexOf(weights.ToList().First());

                        CurrentPack.Banknotes[index] = new Tuple<Banknote, int>(CurrentPack.Banknotes[index].Item1,
                                                                                CurrentPack.Banknotes[index].Item2 - 1);

                        if (CurrentPack.Banknotes[index].Item2 == 0)
                        {
                            CurrentPack.Banknotes.Remove(CurrentPack.Banknotes[index]);
                            NBanknotesKinds--;
                        }

                    }
                    
                    FullSumm -= RequestedSumm;

                    Banknotes.Sort();
                    
                    WithdrawnMoney.Banknotes = CalculateBanknotes();
                }

                catch (InvalidOperationException)
                {
                    return new Tuple<List<Tuple<Banknote, int>>, int, string>(null,
                        CurrentClient.CurrentSumm, Info.ErrorNoCombination);
                }

                return new Tuple<List<Tuple<Banknote, int>>, int, string>(WithdrawnMoney.Banknotes,
                    CurrentClient.CurrentSumm - RequestedSumm, Info.Successed);
            }

            return WithdrawMoney(ref summ);
        }

        public static bool GotRequiredCombination(int residue, ref int counter)
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

        public static List<Tuple<Banknote, int>> CalculateBanknotes()
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
            try
            {
                using (TextWriter writer = new StreamWriter(Info.PackPath))
                {
                    foreach (var tuple in CurrentPack.Banknotes)
                    {
                        writer.WriteLine("{0} {1}", tuple.Item1.Weight, tuple.Item2);
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
