namespace LaplaceEquation
{
    internal class Program
    {
        // Задача номер 18 из файла
        static void Main()
        {   
            int w = 10, h = 20, accuracy = 8;
            var matrix = new double[w, h];

            #region Matrix fill
            for (int i = 0; i < w; i++)
            {
                matrix[i, 0] = i;
            }
            
            for (int i = 0; i < h; i++)
            {
                matrix[0, i] = i;
            }

            for (int i = w - 1; i >= w - h; i--)
            {
                matrix[w - 1, (w - 1) - i] = i;
            }

            for (int i = 1; i < w - 1; i++)
            {
                matrix[i, h - 1] = Math.Round(matrix[0, h - 1] - i * ((matrix[0, h - 1] - matrix[w - 1, h - 1]) / w), 3);
            }
            #endregion Matrix fill
            PrintMatrix(matrix);

            var approximatedMatrix = new GridCalculator().Calculate(matrix, accuracy);
            PrintMatrix(approximatedMatrix);
        }

        static void PrintMatrix(double[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write($"{matrix[i, j]} ");
                }
                Console.WriteLine();
            }
        }
    }
}