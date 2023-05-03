namespace LaplaceEquation;

public class GridApproximator  
{
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private int _accuracyDigits;
    private decimal _accuracy = 1M;
    private int _width { get; init; }
    private int _height { get; init; }

    public decimal[,] Matrix { get; init; }
    public int Accuracy 
    { 
        get => _accuracyDigits;
        init
        {
            if (value < 0)
                throw new ArgumentException("Accuracy can't be less than 0.");

            else if (value > 28)
                throw new ArgumentException("Accurate can't be beyond 28 decimal digits.");

            else
            {
                _accuracyDigits = value;
                for (int i = 0; i < value; i++)
                    _accuracy *= 0.1M;
            }
        }
    }

    public GridApproximator(decimal[,] matrix, int accuracy)
    {
        Matrix = matrix;
        _width = Matrix.GetLength(0);
        _height = Matrix.GetLength(1);
        Accuracy = accuracy;
    }

    /// <summary>
    /// Запускает приближение.
    /// </summary>
    public void StartApproximation()
    {
        ThreadPool.GetMinThreads(out int threadsCount, out _);
        var partWidth = (int)Math.Max(1, Math.Ceiling((decimal)_width / threadsCount));
        var ct = _cancellationTokenSource.Token;
        var tasks = new List<Task>();

        for (int i = 0; i < threadsCount; i++)
        {
            var i1 = i * partWidth;

            if (i1 < _width)
                tasks.Add(Task.Factory.StartNew(() => _partUpdate(i1, partWidth, ct)));

            else 
                break;
        }

        for (int i = 0; i < tasks.Count; i++)
            tasks[i].Wait();
    }

    private void _partUpdate(int startIndex, int length, CancellationToken cancellationToken)
    {
        while (!_isAccuracy(cancellationToken))
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                if (i != 0 && i < _width - 1)
                {
                    for (int j = 1; j < _height - 1; j++)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                            Matrix[i, j] = Math.Round(_getNeighborAverage(i, j), Accuracy);
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
                for (int i = 1; i < _width - 1; i++)
                {
                    for (int j = 1; j < _height - 1; j++)
                    {
                        var current = Matrix[i, j];
                        var expected = _getNeighborAverage(i, j);

                        if (Math.Abs(expected - current).CompareTo(_accuracy) == 1)
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
        return (Matrix[i - 1, j] + Matrix[i, j - 1] + Matrix[i + 1, j] + Matrix[i, j + 1]) / 4;
    }
}
