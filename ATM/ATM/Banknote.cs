namespace ATM
{
    public class Banknote
    {
        public int Weight { get; private set; }
        
        public Banknote(int weight)
        {
            Weight = weight;
        }
    }
}