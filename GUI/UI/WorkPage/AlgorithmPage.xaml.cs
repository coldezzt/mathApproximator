using System.Reflection;
using ApproximatorInterface;

namespace UI;

public partial class AlgorithmPage : ContentPage
{
    public static CalculationStatus CalculationStatus = CalculationStatus.Waiting;

	public AlgorithmPage()
	{
        InitializeComponent();
    }

	public async void ToPreviousPage(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}

    public void UpdateImage(object sender, EventArgs e)
    {
        var dataConfig = Path.GetFileNameWithoutExtension(Settings.PathToAlgorithmData) + "-" + Settings.AlgorithmAccuracy;
        if (Settings.Theme == AppTheme.Light)
        {
            imagePlace0.Source = $@"GraphImages\graph0{dataConfig}.png";
            imagePlace1.Source = $@"GraphImages\graph1{dataConfig}.png";
            imagePlace2.Source = $@"GraphImages\graph2{dataConfig}.png";
            imagePlace3.Source = $@"GraphImages\graph3{dataConfig}.png";
        }
        else
        {
            imagePlace0.Source = $@"GraphImages\graph0inverted{dataConfig}.png";
            imagePlace1.Source = $@"GraphImages\graph1inverted{dataConfig}.png";
            imagePlace2.Source = $@"GraphImages\graph2inverted{dataConfig}.png";
            imagePlace3.Source = $@"GraphImages\graph3inverted{dataConfig}.png";
        }
        
        if (CalculationStatus == CalculationStatus.CompletedNotShown)
            CalculationStatus = CalculationStatus.CompletedShown;
    }

    public async void StartCalculations(object sender, EventArgs e)
    {
        if (AlgorithmConfigurationPage.IsDataSended &&
           (CalculationStatus == CalculationStatus.Waiting ||
            CalculationStatus == CalculationStatus.CompletedShown))
        {
            CalculationStatus = CalculationStatus.Working;
            pageRefreshButton.IsEnabled = false;

            var exitCode = await Task.Run(Calculate);

            if (exitCode == ExitCode.FileError)
            {
                await DisplayAlert("Ошибка в файле", "Ошибка во время чтения файла. " +
                        "Проверьте целостность файла.\nНаправляю на страницу с " +"конфигурацией.", "Ок");
                await Navigation.PopAsync();
            }
            else if (exitCode == ExitCode.LibraryError)
            {
                await DisplayAlert("Ошибка обработки библеотеки", "В предоставленной библеотеке " +
                        "отсутствует класс реализующий интерфейс IApproximator. Проверьте путь к " +
                        "предоставленной библеотеке и попробуйте ещё раз.\nНаправляю на страницу с " +
                        "конфигурацией.", "Ок");
                await Navigation.PopAsync();
            }
            else
            {
                info.Text = "Результаты готовы";
                pageRefreshButton.IsEnabled = true;
                CalculationStatus = CalculationStatus.CompletedNotShown;
            }
        }
        else if (CalculationStatus == CalculationStatus.CompletedNotShown)
        {
            info.Text = "Результаты готовы";
            pageRefreshButton.IsEnabled = true;
        }
    }

    private ExitCode Calculate()
    {
        Assembly calculatingLibrary = Assembly.LoadFrom(Settings.PathToCalculatingLibrary);

        var approximator = GetApproximatorFromLibrary(calculatingLibrary);        

        if (approximator == null)
            return ExitCode.LibraryError;

        var matrix = CreateMatrixFromData();
        if (matrix == null)
            return ExitCode.FileError;

        var result = approximator.Approximate(matrix, Settings.AlgorithmAccuracy);
        SaveAlgorithmResults(result);
        Settings.CreateGnuplotScript();
        Settings.CreateImagesWithGnuplot();

        return ExitCode.Completed;
    }
    private IApproximator GetApproximatorFromLibrary(Assembly library)
    {
        try
        {
            IApproximator approximator = 
                (IApproximator)library.CreateInstance
                (
                    library.GetTypes()
                           .First(x => 
                                  x.GetInterfaces()
                                   .Contains(typeof(IApproximator)) &&
                                 !x.IsInterface)
                                   .ToString()
                );

            return approximator;
        }
        catch
        {
            return null;
        }
        /*
        Type[] types = library.GetTypes();
        foreach (Type type in types)
        {
            if (type.GetInterfaces().Contains(typeof(IApproximator)))
            {
                var instance = library.CreateInstance(type.FullName);
                var approximator = (IApproximator)instance;
                return approximator;
            }
        }
        return null;
        */
    }
    private decimal[,] CreateMatrixFromData()
    {
        decimal[,] matrix;
        try
        {
            using (var sr = new StreamReader(Settings.PathToAlgorithmData))
            {
                string[] numbersStrings =
                    sr.ReadToEnd()
                      .Replace('.', ',')
                      .Trim()
                      .Split("\r\n");

                var width = numbersStrings.Length;
                var length = numbersStrings[0].Trim().Split(' ').Count();

                matrix = new decimal[width, length];

                for (int i = 0; i < width; i++)
                {
                    var nums = numbersStrings[i].Trim().Split(' ');

                    for (int j = 0; j < length; j++)
                    {
                        matrix[i, j] = Convert.ToDecimal(nums[j]);
                    }
                }
            }
        }
        catch
        {
            return null;
        }

        return matrix;
    }
    private void SaveAlgorithmResults(decimal[,] matrix)
    {
        using (var sw = new StreamWriter(Settings.PathToAlgorithmResults))
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    sw.Write($"{matrix[i, j]} ");
                }
                sw.WriteLine();
            }
        }
    }
}