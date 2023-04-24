using System.Numerics;

namespace LaplaceEquation;

public class GridApproximator
{
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private int _accuracyDigits;
    private decimal _accuracy = 1M;
    public decimal[,] Matrix { get; init; }
    public int Accuracy 
    { 
        get => _accuracyDigits;
        init
        {
            if (value < 0)
                throw new ArgumentException("Accuracy can't be less than 0.");

            else if (value > 28)
                throw new ArgumentException("It is impossible to be accurate beyond 28 decimal digits.");

            else
            {
                _accuracyDigits = value;
                for (int i = 0; i < value; i++)
                    _accuracy *= 0.1M;
            }
        }
    }

    private int _width => Matrix.GetLength(0);
    private int _height => Matrix.GetLength(1);

    public GridApproximator(decimal[,] matrix, int accuracy)
    {
        Matrix = matrix;
        Accuracy = accuracy;
    }

    public void StartApproximation()
    {
        ThreadPool.GetMinThreads(out int threadsCount, out _);
        var partWidth = (int)Math.Max(1, Math.Ceiling((decimal)_width / threadsCount));
        var ct = _cancellationTokenSource.Token;

        var tasks = new List<Task>();

        // Распределяем ресурсы, запускаем работу
        for (int i = 0; i < threadsCount; i++)
        {
            var i1 = i * partWidth;

            if (i1 < _width)
                tasks.Add(Task.Factory.StartNew(() => _partUpdate(i1, partWidth, ct)));

            else 
                break;
        }

        // Ждём старта завершения работы
        while (!ct.IsCancellationRequested) ;

        // Ждём конца завершения работы
        for (int i = 0; i < tasks.Count; i++)
            tasks[i].Wait();
    }

    private void _partUpdate(int startIndex, int length, CancellationToken cancellationToken)
    {
        // Пока функция не скажет что всё ок продолжаем работать
        while (!_isAccuracy(cancellationToken))
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                /* Границы нельзя менять по идее. Потому что, во-первых, у них не хватает
                 * соседей, а, во-вторых, изменение границ ведёт к изменению того, как
                 * итоговая поверхность будет пересакать границы => не будет совпадать с
                 * изначальной поверхностью.
                 */
        
                if (i != 0 && i != _width - 1)
                {
                    for (int j = 1; j < _height - 1; j++)
                    {
                        // Чтобы не было лишней замены вдруг
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        // Обновление значения
                        Matrix[i, j] = Math.Round(_getNeighborAverage(i, j), Accuracy);
                    }
                }
            }
        }
    }

    private bool _isAccuracy(CancellationToken cancellationToken)
    {
        // Крит точка
        var locker = new object();
        lock (locker)
        {
            // Собралась очередь -> первый прошёлся если вернуло true -> можно заканчивать работу 
            if (!cancellationToken.IsCancellationRequested)
            {
                for (int i = 1; i < _width - 1; i++)
                {
                    for (int j = 1; j < _height - 1; j++)
                    {
                        var current = Matrix[i, j];
                        var expected = _getNeighborAverage(i, j);

                        // Проверка на совпадение
                        if (Math.Abs(expected - current).CompareTo(_accuracy) > -1)
                            return false;
                    }
                }
            }

            // Заканчиваем работу
            _cancellationTokenSource.Cancel();
            return true;
        }
    }

    private decimal _getNeighborAverage(int i, int j)
    {
        // Средние среди 4 значений
        return (Matrix[i - 1, j] + Matrix[i, j - 1] + Matrix[i + 1, j] + Matrix[i, j + 1]) / 4;
    }
}
