using ApproximatorInterface;

namespace LaplaceEquation;

public class AreaSurfaceApproximator : IApproximator
{
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private int _accuracyDigits = 0;
    private int _width = 0;
    private int _height = 0;
    private decimal _minDeviation = 1M;
    private decimal[,] _resultMatrix = new decimal[0, 0];

    public decimal[,] Approximate(decimal[,] matrix, int accuracy)
    {
        Initialize(matrix, accuracy);

        ThreadPool.GetMinThreads(out int threadsCount, out _);
        var partWidth = (int)Math.Max(1, Math.Ceiling((decimal)_width / threadsCount));

        var ct = _cancellationTokenSource.Token;
        var tasks = new List<Task>();

        for (int i = 0; i < threadsCount; i++)
        {
            var i1 = i * partWidth;

            if (i1 < _width)
                tasks.Add(Task.Factory.StartNew(() => PartUpdate(i1, partWidth, ct)));

            else break;
        }

        for (int i = 0; i < tasks.Count; i++)
            tasks[i].Wait();

        return _resultMatrix;
    }
    
    private void Initialize(decimal[,] matrix, int accuracy)
    {
        if (matrix == null)
            throw new ArgumentNullException(nameof(matrix));

        if (accuracy < 0)
            throw new ArgumentException("Accuracy can't be less than 1.");

        else if (accuracy > 28)
            throw new ArgumentException("Accurate can't be beyond 28 decimal digits.");


        _resultMatrix = matrix;
        _width = _resultMatrix.GetLength(0);
        _height = _resultMatrix.GetLength(1);

        _accuracyDigits = accuracy;

        for (int i = 0; i < accuracy; i++)
            _minDeviation *= 0.1M;
    }
    private void PartUpdate(int startIndex, int length, CancellationToken cancellationToken)
    {
        while (!IsAccuracy(cancellationToken))
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                if (i != 0 && i < _width - 1)
                {
                    for (int j = 1; j < _height - 1; j++)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                            _resultMatrix[i, j] = Math.Round(GetNeighborAverage(i, j), _accuracyDigits);
                    }
                }
            }
        }
    }
    private bool IsAccuracy(CancellationToken cancellationToken)
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
                        var current = _resultMatrix[i, j];
                        var expected = GetNeighborAverage(i, j);

                        if (Math.Abs(expected - current).CompareTo(_minDeviation) == 1)
                            return false;
                    }
                }
            }

            _cancellationTokenSource.Cancel();
            return true;
        }
    }
    private decimal GetNeighborAverage(int i, int j)
    {
        return (_resultMatrix[i - 1, j] + _resultMatrix[i, j - 1] + _resultMatrix[i + 1, j] + _resultMatrix[i, j + 1]) / 4;
    }
}
