using System.Numerics;

namespace LaplaceEquation;

public class GridApproximator
{
    public double[,] Matrix { get; init; }
    public int Accuracy { get; init; }

    private int _width => Matrix.GetLength(0);
    private int _height => Matrix.GetLength(1);

    public GridApproximator(double[,] matrix, int accuracy)
    {
        Matrix = matrix;
        Accuracy = accuracy;
    }

    public void StartApproximation()
    {
        ThreadPool.GetMinThreads(out int threadsCount, out _);
        var partWidth = (int)Math.Max(1, Math.Ceiling((double)_width / threadsCount));

        var tasks = new List<Task>();

        for (int i = 0; i < threadsCount; i++)
        {
            var i1 = i * partWidth;

            if (i1 < _width)
                tasks.Add(Task.Factory.StartNew(() => _partUpdate(i1, partWidth)));

            else break;
        }


        bool IsAllComplete = false;
        while (!IsAllComplete)
        {
            int completedTasks = 0;
            foreach (var task in tasks)
            {
                if (task.IsCompleted)
                    completedTasks++;

                else
                    break;
            }

            if (completedTasks == tasks.Count)
                IsAllComplete = true;
        }
    }

    private void _partUpdate(int startIndex, int length)
    {
        while (!_isAccuracy())
        {
            for (int i = startIndex; i < startIndex + length; i++)
            {
                // Границы нельзя менять по идее. Потому что, во-первых, у них не хватает
                // соседей, а, во-вторых, изменение границ ведёт к изменению того, как
                // итоговая поверхность будет пересакать границы => не будет совпадать с
                // изначальной поверхностью.

                if (i != 0 && i != _width - 1)
                {
                    for (int j = 1; j < _height - 1; j++)
                    {
                        Matrix[i, j] = Math.Round(_getNeighborAverage(i, j), Accuracy);
                    }
                }
            }
        }
    }

    private bool _isAccuracy()
    {
        for (int i = 1; i < _width - 1; i++)
        {
            for (int j = 1; j < _height - 1; j++)
            {
                var current = Matrix[i, j];
                var expected = _getNeighborAverage(i, j);

                if (Math.Abs(expected - current) > Math.Pow(0.1, Accuracy))
                    return false;
            }
        }
        return true;
    }

    private double _getNeighborAverage(int i, int j)
    {
        return (Matrix[i - 1, j] + Matrix[i, j - 1] + Matrix[i + 1, j] + Matrix[i, j + 1]) / 4;
    }
}
