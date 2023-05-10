namespace UI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            Directory.SetCurrentDirectory(Path.GetDirectoryName(typeof(App).Assembly.Location));
            Directory.CreateDirectory("GraphImages");
            File.Create("gnuplot_script.p");
            Settings.SetSettings();
            MainPage = new NavigationPage(new MainPage());
        }
    }
}