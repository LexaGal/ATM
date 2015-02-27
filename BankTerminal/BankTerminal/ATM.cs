using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BankTerminal
{
    public partial class ATM : Form
    {
        Bank bank = new Bank(int.MaxValue);

        public ATM()
        {
            InitializeComponent();
        }

        private void AtmLoad(object sender, EventArgs e)
        {
            bank.CreateBanknote(10, 55);
            bank.CreateBanknote(20, 55);
            bank.CreateBanknote(50, 55);
            bank.CreateBanknote(100, 55);
            bank.CreateBanknote(200, 55);
        }

        private void GetBanknotes(object sender, EventArgs e)
        {
            try
            {
                tbDivision.Clear();
                Client client = new Client(tbPIN.Text, 100000);
                BankTerminal.InsertBanknotes(bank.CreateBanknotesPack());
                BankTerminal.ServiceNewClient(new Client(client));
                int summ = int.Parse(tbMoney.Text);
                Tuple<int, string> result = new Tuple<int, string>(0, null);

                result = BankTerminal.MoneyWithdrawn(ref summ);

                if (result.Item1 == -1)
                {
                    MessageBox.Show(result.Item2);
                    return;
                }

                BankTerminal.BanknotesOut.OrderByDescending(tuple => tuple.Item1.Weight).Where(elem1 => elem1.Item2 != 0)
                    .ToList().ForEach(elem2 =>
                                      tbDivision.Text +=
                                      elem2.Item1.Weight.ToString() + '*' + elem2.Item2.ToString() + " + ");

                tbDivision.Text = tbDivision.Text.Remove(tbDivision.Text.Count() - 3, 3);
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
