﻿using System;
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
            @"C:\Users\Alex\Desktop\ATM\ATM\Pack.txt";

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

        public static Tuple<int, string> WithdrawMoney(ref int summ)
        {
            if (summ > FullSumm)
                return new Tuple<int, string>(-1, "Sorry, you cannot get such summ (not enough money in ATM)");

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

            //tuple with max probability
            if (NBanknotesKinds == 0)
                return new Tuple<int, string>(-1, "Sorry, you cannot get such summ (not enough money in ATM)");

            var max = new Tuple<Banknote, double>(probabilities[NBanknotesKinds - 1].Item1,
                                                  probabilities[NBanknotesKinds - 1].Item2);

            BanknotesOut.Add(max.Item1.Weight);

            //index of max in banknotes out
            var index = WithdrawnMoney.BanknotesOut.FindIndex(tuple => tuple.Item1.Weight == max.Item1.Weight);


            //increase n of banknotes out of this weight
            WithdrawnMoney.BanknotesOut[index] = new Tuple<Banknote, int>(WithdrawnMoney.BanknotesOut[index].Item1,
                                                                          WithdrawnMoney.BanknotesOut[index].Item2 + 1);

           //decrease n of banknotes of this weight in current pack
            CurrentPack.Banknotes[index] = new Tuple<Banknote, int>(CurrentPack.Banknotes[index].Item1,
                                                                    CurrentPack.Banknotes[index].Item2 - 1);

            if (CurrentPack.Banknotes[index].Item2 == 0)
            {
                CurrentPack.Banknotes.Remove(CurrentPack.Banknotes[index]);
                NBanknotesKinds--;
            }

            summ -= max.Item1.Weight;

            if (summ == 0)
            {
                FullSumm -= RequestedSumm;
                return new Tuple<int, string>(CurrentClient.CurrentSumm - RequestedSumm, "Successed");
            }

            if (summ < CurrentPack.Banknotes.Min(tuple => tuple.Item1.Weight) && CurrentPack.Banknotes.Count != 0)
            {
                do
                {
                    WithdrawnMoney.BanknotesOut.Clear();
                    
                    summ += BanknotesOut.Last();

                    var elem = CurrentPack.Banknotes.Where(el => el.Item1.Weight == BanknotesOut.Last());

                    index = CurrentPack.Banknotes.IndexOf(elem.ToList()[0]);
                    
                    CurrentPack.Banknotes[index] = new Tuple<Banknote, int>(CurrentPack.Banknotes[index].Item1,
                                                                            CurrentPack.Banknotes[index].Item2 + 1);
                    
                    BanknotesOut.Remove(BanknotesOut.Last());
                    
                } while (!BanknotesChanged(summ));
                
                FullSumm -= RequestedSumm;

                return new Tuple<int, string>(CurrentClient.CurrentSumm - RequestedSumm, "Successed");
            }

            return WithdrawMoney(ref summ);
        }

        public static bool BanknotesChanged(int residue)
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
            }

            BanknotesOut.Sort();
            return true;
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
