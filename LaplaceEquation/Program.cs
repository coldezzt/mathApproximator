using System.Diagnostics;

namespace LaplaceEquation
{
    internal class Program
    {
        // Задача номер 18 из файла
        static void Main()
        {   
            // Ширина, высота, точность (знаки после запятой)
            int w = 50, h = 100, accuracy = 28;
            var matrix = new decimal[w, h];

            // Заполнение матрицы (моими функциями)
            // В будущем сделаю рандомным заполнением (если возможно???)
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

            using (var sw = new StreamWriter("..\\..\\..\\results.txt"))
            {
                for (int i = 0; i < w; i++)
                {
                    for (int j = 0; j < h - 1; j++)
                    {
                        sw.WriteLine($"{i} {j} {gridApproximator.Matrix[i, j]}");
                    }
                }
                sw.WriteLine($"{w - 1} {h - 1} {gridApproximator.Matrix[w - 1, h - 1]}");
            }
        }
    }
}