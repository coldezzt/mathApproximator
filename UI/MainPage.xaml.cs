using CommunityToolkit.Maui.Extensions;

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
            await ButtonClicked(sender as Button);
            await Navigation.PushAsync(new WorkingPage());
        }

        async public void ToInstructionsPage(object sender, EventArgs e)
        {
            await ButtonClicked(sender as Button);
            await Navigation.PushAsync(new InstructionsPage());
        }
        async public void ToSettingsPage(object sender, EventArgs e)
        {
            await ButtonClicked(sender as Button);
            await Navigation.PushAsync(new SettingsPage());
        }

        async public void Quit(object sender, EventArgs e)
        {
            await ButtonClicked(sender as Button);
            var accepted = await DisplayAlert("Подтвердить действие", "Вы уверены что хотите выйти?", "Да", "Нет");
            if (accepted)
                App.Current.Quit();
        }

        async private Task<int> ButtonClicked(Button btn)
        {
            await btn.BackgroundColorTo(Color.FromArgb("666666"), 1, 100);
            await btn.BackgroundColorTo(Color.FromArgb("FFFFFF"), 1, 100);
            return 0;
        }
    }
}