
namespace ATM.Classes
{
    public class Info
    {
        public static string BanknotesPackPath
        {
            get { return @"C:\Users\Alex\Documents\GitHub\ATM\ATM\ATM\ATM\bin\Debug\BanknotesFile.txt"; }
        }

        public static string ClientsFilePath
        {
            get { return @"C:\Users\Alex\Documents\GitHub\ATM\ATM\ATM\ATM\bin\Debug\ClientsFile.txt"; }
        }

        public static string OperationErrorNotEnoughMoneyClient
        {
            get { return "Sorry, you cannot get such summ (you do not have enough money)"; }
        }

        public static string OperationErrorNotEnoughMoneyATM
        {
            get { return "Sorry, you cannot get such summ (not enough money in ATM)"; }
        }

        public static string OperationErrorNoBanknotesCombination
        {
            get { return "Sorry, you cannot get such summ (ATM cannot complete your summ)"; }
        }

        public static string OperationSuccessed
        {
            get { return "Operation Successed"; }
        }

    }
}