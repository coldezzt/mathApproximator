using System.Diagnostics;
using System.Dynamic;

namespace LaplaceEquation;

public class GridApproximator
{
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private int _accuracyDigits;
    private decimal _accuracy = 1M;
    private string pathToResult = "";

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

        // Распределяем ресурсы, запускаем работу
        for (int i = 0; i < threadsCount; i++)
        {
            var i1 = i * partWidth;

            if (i1 < _width)
                tasks.Add(Task.Factory.StartNew(() => _partUpdate(i1, partWidth, ct)));

            else 
                break;
        }

        // Ждём конца завершения работы
        for (int i = 0; i < tasks.Count; i++)
            tasks[i].Wait();
    }

    /// <summary>
    /// Сохраняет результаты. Принимает на вход путь к файлу, в который нужно сохранить данные.<br/>
    /// Сохраняет путь к файлу для отображения его в функции ShowResult.
    /// </summary>
    /// <param name="path">Путь до файла.</param>
    public void SaveResult(string path)
    {
        using (var sw = new StreamWriter(path))
        {
            pathToResult = path;
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    sw.WriteLine($"{i} {j} {Matrix[i, j]}");
                }
            }
        }
    }
    
    /// <summary>
    /// Показывает обработанный график по пути указанному в функции SaveResult.
    /// </summary>
    /// <param name="pathToGnuplot">Путь до gnuplot.</param>
    /// <param name="scriptPath">Путь до скрипта.</param>
    public void ShowResult(string pathToGnuplot, string scriptPath = @"..\..\..\gnuplot_script.p")
    {
        // Если результаты не были сначала сохранены то ничего не делаем (надо довести до ума)
        if (pathToResult == string.Empty)
            return;

        // Создание скрипта для gnuplot
        using (var sw = new StreamWriter(scriptPath))
        {
            sw.WriteLine($@"splot '{pathToResult}'");
            sw.WriteLine("print 'Press enter to exit.'");
            sw.WriteLine("pause -1");
        }

        Process.Start(pathToGnuplot, scriptPath);
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
        
                if (i != 0 && i < _width - 1)
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

                        if (Math.Abs(expected - current).CompareTo(_accuracy) == 1)
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
        return (Matrix[i - 1, j] + Matrix[i, j - 1] + Matrix[i + 1, j] + Matrix[i, j + 1]) / 4;
    }
}
