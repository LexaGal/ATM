using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ATM
{
    public class Actor
    {
        static void Main()
        {
            try
            {
                var bank = new Bank(int.MaxValue);

                var pack = bank.CreateBanknotesPack();
                BankTerminal.InsertBanknotes(pack);
                
                while (true)
                {
                    var client = new Client();
                    BankTerminal.ServiceNewClient(client);

                    //-------------------------------

                    Console.WriteLine("\nSumm in ATM: {0}", BankTerminal.FullSumm);
                    Console.WriteLine("Client's current summ: {0}", client.CurrentSumm);

                    Console.Write("Input the summ to get: ");
                    var money = Console.ReadLine();
                    var summ = 0;

                    if (!String.IsNullOrEmpty(money))
                        summ = int.Parse(money);

                    if (summ == -1) break;

                    //-------------------------------

                    var gotMoney = client.GetMoney(summ);

                    if (gotMoney != null)
                    {
                        Console.WriteLine(gotMoney);
                    }
                    
                    else
                    {
                        var banknotes = BankTerminal.BanknotesOut.OrderByDescending(el => el).ToList();
                        var tuples = new List<Tuple<Banknote, int>>();
                        var result = new StringBuilder("\n");

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

                        tuples.Where(elem1 => elem1.Item2 != 0).ToList().ForEach(elem2 =>
                        result.Append(elem2.Item1.Weight.ToString() + " * " + elem2.Item2.ToString() + " + "));
                        
                        result.Remove(result.ToString().Count() - 3, 3);
            
                        
                        Console.WriteLine(result);
                    }

                    client.SaveState();
                    BankTerminal.Finish();
                }
            }

            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
