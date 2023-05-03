namespace LaplaceEquation;

/// <summary>
/// Достраивает поверхность внутри по краям полученной матрицы.
/// </summary>
public static class AreaSurfaceApproximator
{
    static private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    static private int accuracyDigits = 0;
    static private int width = 0;
    static private int height = 0;
    static private decimal minDeviation = 1M;
    static private decimal[,] resultMatrix = new decimal[0, 0];

    /// <param name="matrix">Матрица для приближения</param>
    /// <param name="accuracy">Точность приближения</param>
    static public decimal[,] Approximate(decimal[,] matrix, int accuracy)
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
    
    static private void _initialize(decimal[,] matrix, int accuracy)
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
    static private void _partUpdate(int startIndex, int length, CancellationToken cancellationToken)
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
    static private bool _isAccuracy(CancellationToken cancellationToken)
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
    static private decimal _getNeighborAverage(int i, int j)
    {
        return (resultMatrix[i - 1, j] + resultMatrix[i, j - 1] + resultMatrix[i + 1, j] + resultMatrix[i, j + 1]) / 4;
    }
}
