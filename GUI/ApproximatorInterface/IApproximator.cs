namespace Contract;

public interface IApproximator
{
    public decimal[,] Approximate(decimal[,] matrix, int accuracy);
}