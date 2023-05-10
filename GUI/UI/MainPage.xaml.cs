namespace UI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async public void ToWorkingPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AlgorithmConfigurationPage());
        }

        async public void ToSettingsPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        async public void Quit(object sender, EventArgs e)
        {
            var accepted = await DisplayAlert("Подтвердить действие", "Вы уверены что хотите выйти?", "Да", "Нет");
            if (accepted)
                Application.Current.Quit();
        }   
    }
}