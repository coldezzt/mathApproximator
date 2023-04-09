namespace LaplaceEquation;

public class GridCalculator
{

    /// <summary>
    /// Что приближать, и до какого знака после запятой
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="accuracy"></param>
    /// <returns></returns>
    public double[,] Calculate(double[,] matrix, int accuracy)
    {
        ThreadPool.GetMinThreads(out int threadsCount, out _);
        int width = matrix.GetLength(0), height = matrix.GetLength(1);

        var matrixPartWidth = (int)Math.Max(1, Math.Ceiling((double)width / threadsCount));

        for(int i = 0; i < threadsCount; i++)
        {
            var i1 = i * matrixPartWidth;
            
            if (i1 < width)
                new Thread(() => PartUpdate(i1, matrixPartWidth)).Start();
            
            else break;
        }

        while (!IsAccuracy()) ;

        return matrix;

        void PartUpdate(int startIndex, int length)
        {
            var endIndex = startIndex + length;

            while (!IsAccuracy())
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    // Границы нельзя менять по идее. Иначе, в конечном итоге, мы получим просто плоскость
                    // (если программа будет достаточно долго работать)
                    if (i != 0 && i != width - 1)
                    {
                        for (int j = 1; j < height - 1; j++)
                        {
                            matrix[i, j] = Math.Round(GetNeighborAverage(i, j), accuracy);
                        }
                    }
                }
            }
        }
    
        bool IsAccuracy()
        {
            for (int i = 1; i < width - 1; i++)
            {
                for (int j = 1; j < height - 1; j++)
                {
                    var current = matrix[i, j]; 
                    var expected = GetNeighborAverage(i, j);

                    if (Math.Abs(expected - current) > Math.Pow(0.1, accuracy))
                        return false;
                }
            }
            return true;
        }

        double GetNeighborAverage(int i, int j)
        {
            return (matrix[i - 1, j] + matrix[i, j - 1] + matrix[i + 1, j] + matrix[i, j + 1]) / 4;
        }
    }
}
