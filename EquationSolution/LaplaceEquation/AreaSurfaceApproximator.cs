using ApproximatorInterface;

namespace LaplaceEquation;

public class AreaSurfaceApproximator : IApproximator
{
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private int accuracyDigits = 0;
    private int width = 0;
    private int height = 0;
    private decimal minDeviation = 1M;
    private decimal[,] resultMatrix = new decimal[0, 0];

    public decimal[,] Approximate(decimal[,] matrix, int accuracy)
    {
        _initialize(matrix, accuracy);

        ThreadPool.GetMinThreads(out int threadsCount, out _);
        var partWidth = (int)Math.Max(1, Math.Ceiling((decimal)width / threadsCount));

        var ct = _cancellationTokenSource.Token;
        var tasks = new List<Task>();

        for (int i = 0; i < threadsCount; i++)
        {
            var i1 = i * partWidth;

            if (i1 < width)
                tasks.Add(Task.Factory.StartNew(() => _partUpdate(i1, partWidth, ct)));

            else 
                break;
        }

        for (int i = 0; i < tasks.Count; i++)
            tasks[i].Wait();

        return resultMatrix!;
    }
    
    private void _initialize(decimal[,] matrix, int accuracy)
    {
        if (matrix == null)
            throw new ArgumentNullException(nameof(matrix));

        if (accuracy < 0)
            throw new ArgumentException("Accuracy can't be less than 1.");

        else if (accuracy > 28)
            throw new ArgumentException("Accurate can't be beyond 28 decimal digits.");


        resultMatrix = matrix;
        width = resultMatrix.GetLength(0);
        height = resultMatrix.GetLength(1);

        accuracyDigits = accuracy;
        for (int i = 0; i < accuracy; i++)
            minDeviation *= 0.1M;
    }
    private void _partUpdate(int startIndex, int length, CancellationToken cancellationToken)
    {
        while (!_isAccuracy(cancellationToken))
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                if (i != 0 && i < width - 1)
                {
                    for (int j = 1; j < height - 1; j++)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                            resultMatrix[i, j] = Math.Round(_getNeighborAverage(i, j), accuracyDigits);
                    }
                }
            }
        }
    }
    private bool _isAccuracy(CancellationToken cancellationToken)
    {
        var locker = new object();
        lock (locker)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                for (int i = 1; i < width - 1; i++)
                {
                    for (int j = 1; j < height - 1; j++)
                    {
                        var current = resultMatrix[i, j];
                        var expected = _getNeighborAverage(i, j);

                        if (Math.Abs(expected - current).CompareTo(minDeviation) == 1)
                            return false;
                    }
                }
            }

            _cancellationTokenSource.Cancel();
            return true;
        }
    }
    private decimal _getNeighborAverage(int i, int j)
    {
        return (resultMatrix[i - 1, j] + resultMatrix[i, j - 1] + resultMatrix[i + 1, j] + resultMatrix[i, j + 1]) / 4;
    }
}
