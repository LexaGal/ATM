namespace ATM
{
    public class Info
    {
        public static string PackPath
        {
            get { return @"C:\Users\Alex\Documents\GitHub\ATM\ATM\ATM\ATM\bin\Debug\Pack.txt"; }
        }

        public static string ClientsPath
        {
            get { return @"C:\Users\Alex\Documents\GitHub\ATM\ATM\ATM\ATM\bin\Debug\Clients.txt"; }
        }

        public static string ErrorNoMoneyClient
        {
            get { return "Sorry, you cannot get such summ (you do not have enough money)"; }
        }

        public static string ErrorNoMoneyATM
        {
            get { return "Sorry, you cannot get such summ (not enough money in ATM)"; }
        }

        public static string ErrorNoCombination
        {

            get { return "Sorry, you cannot get such summ (ATM cannot complete your summ)"; }
        }

        public static string Successed
        {
            get { return "Successed"; } 
        }

    }
}