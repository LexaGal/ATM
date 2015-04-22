
namespace ATM.Classes
{
    public class Info
    {
        public static string BanknotesPackPath
        {
            get { return @"C:\Users\Alex\Documents\GitHub\ATM\ATM\ATM\ATM\BanknotesFile.txt"; }
        }

        public static string ClientsFilePath
        {
            get { return @"C:\Users\Alex\Documents\GitHub\ATM\ATM\ATM\ATM\ClientsFile.txt"; }
        }

        public static string OperationErrorNotEnoughMoneyClient
        {
            get { return "You cannot get such sum (you do not have enough money)"; }
        }

        public static string OperationErrorNotEnoughMoneyATM
        {
            get { return "You cannot get such sum (not enough money in ATM)"; }
        }

        public static string OperationErrorNoBanknotesCombination
        {
            get { return "You cannot get such sum (ATM cannot complete your sum)"; }
        }

        public static string OperationSuccessed
        {
            get { return "Operation Successed"; }
        }

    }
}