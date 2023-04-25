namespace LaplaceEquation
{
    internal class Program
    {
        // Задача номер 18 из файла
        static void Main()
        {
            // Ширина, высота, точность (знаки после запятой)
            int w = 100, h = 200, accuracy = 1;
            var matrix = new decimal[w, h];

            // Заполнение матрицы (моими функциями)
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
            
            var gridApproximator = new GridApproximator(matrix, accuracy);
            gridApproximator.StartApproximation();
            gridApproximator.ShowResult(@"..\..\..\..\gnuplot\gnuplot.exe");
        }
    }
}