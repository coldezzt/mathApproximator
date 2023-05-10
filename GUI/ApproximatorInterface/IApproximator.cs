namespace ApproximatorInterface;

public interface IApproximator
{
    public decimal[,] Approximate(decimal[,] matrix, int accuracy);
}