using System.Diagnostics;

namespace UI;

public static class Settings
{
    public static AppTheme Theme
    {
        get { return Application.Current.UserAppTheme; }
         set { Application.Current.UserAppTheme = value; }
    }

    public static int AlgorithmAccuracy { get; set; }
    public static string PathToAlgorithmData { get; set; }
    public static string PathToAlgorithmResults { get; set; }
    
    public static string PathToCalculatingLibrary { get; set; }
    public static string PathToGnuplot { get; set; }
    public static string PathToGnuplotScript{ get; set; }
    public static string PathToSurfaceImages { get; set; }

    public static void SetSettings()
    {
        if (!File.Exists("settings.cfg"))
        {
            PathToAlgorithmData = "";
            PathToAlgorithmResults = "";
            PathToCalculatingLibrary = "";
            PathToGnuplot = @"..\Gnuplot\gnuplot.exe";
            PathToGnuplotScript = @"gnuplot_script.p";
            PathToSurfaceImages = @"GraphImages\";
        }
        else
        {
            UploadConfig();
        }
    }
    public static void UpdateConfig()
    {
        using (var sw = new StreamWriter("settings.cfg")) 
        {
            sw.Write($"{Theme}::");
            sw.Write($"{AlgorithmAccuracy}::");
            sw.Write($"{PathToAlgorithmData}::");
            sw.Write($"{PathToAlgorithmResults}::");
            sw.Write($"{PathToCalculatingLibrary}::");
            sw.Write($"{PathToGnuplot}::");
            sw.Write($"{PathToGnuplotScript}::");
            sw.Write($"{PathToSurfaceImages}");
        }
    }
    public static void UploadConfig()
    {
        try
        {
            using (var sr = new StreamReader("settings.cfg"))
            {
                var config = sr.ReadToEnd().Split("::");

                Theme = config[0] == "Light" ? AppTheme.Light : AppTheme.Dark;
                int.TryParse(config[1], out int accuracy);
                AlgorithmAccuracy = accuracy;
                PathToAlgorithmData = config[2];
                PathToAlgorithmResults = config[3];
                PathToCalculatingLibrary = config[4];
                PathToGnuplot = config[5];
                PathToGnuplotScript = config[6];
                PathToSurfaceImages = config[7];
            }
        }
        catch
        {
            File.Delete("settings.cfg");
            SetSettings();
        }
    }
    public static void CreateGnuplotScript()
    {
        using (var sw = new StreamWriter(PathToGnuplotScript))
        {
            var dataConfig = Path.GetFileNameWithoutExtension(PathToAlgorithmData) + "-" + AlgorithmAccuracy;
            sw.WriteLine($@"set terminal png size 854, 480");

            sw.WriteLine($@"set output 'GraphImages\graph0{dataConfig}.png'");
            sw.WriteLine("set view 90, 0");
            sw.WriteLine($"splot '{PathToAlgorithmResults}' matrix with pm3d");

            sw.WriteLine($@"set output 'GraphImages\graph1{dataConfig}.png'");
            sw.WriteLine("set view 90, 90");
            sw.WriteLine($"splot '{PathToAlgorithmResults}' matrix with pm3d");

            sw.WriteLine($@"set output 'GraphImages\graph2{dataConfig}.png'");
            sw.WriteLine("set view 60, 80");
            sw.WriteLine($"splot '{PathToAlgorithmResults}' matrix with pm3d");

            sw.WriteLine($@"set output 'GraphImages\graph3{dataConfig}.png'");
            sw.WriteLine("set view 60, 260");
            sw.WriteLine($"splot '{PathToAlgorithmResults}' matrix with pm3d");

            sw.WriteLine($@"set object 1 rectangle from screen 0,0 to screen 1,1 fillcolor rgb ""black"" behind");
            sw.WriteLine($@"set border lc rgb ""white""");
            sw.WriteLine($@"set key textcolor rgb ""white""");

            sw.WriteLine($@"set output 'GraphImages\graph0inverted{dataConfig}.png'");
            sw.WriteLine("set view 90, 0");
            sw.WriteLine($"splot '{PathToAlgorithmResults}' matrix with pm3d");

            sw.WriteLine($@"set output 'GraphImages\graph1inverted{dataConfig}.png'");
            sw.WriteLine("set view 90, 90");
            sw.WriteLine($"splot '{PathToAlgorithmResults}' matrix with pm3d");

            sw.WriteLine($@"set output 'GraphImages\graph2inverted{dataConfig}.png'");
            sw.WriteLine("set view 60, 80");
            sw.WriteLine($"splot '{PathToAlgorithmResults}' matrix with pm3d");

            sw.WriteLine($@"set output 'GraphImages\graph3inverted{dataConfig}.png'");
            sw.WriteLine("set view 60, 260");
            sw.WriteLine($"splot '{PathToAlgorithmResults}' matrix with pm3d");
        }
    }
    public static void CreateImagesWithGnuplot()
    {
        Process proc = new Process();
        proc.StartInfo.FileName = $"{PathToGnuplot}";
        proc.StartInfo.Arguments = $"{PathToGnuplotScript}";
        proc.StartInfo.CreateNoWindow = true;
        proc.StartInfo.UseShellExecute = false;
        proc.Start();
    }
}
