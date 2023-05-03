using CommunityToolkit.Maui.Extensions;

namespace UI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async public void ToWorkingPage(object s, EventArgs e)
        {
            await ButtonClicked(s as Button);
            await Navigation.PushAsync(new WorkingPage());
        }

        async public void ToInstructionsPage(object s, EventArgs e)
        {
            await ButtonClicked(s as Button);
            await Navigation.PushAsync(new InstructionsPage());
        }

        async public void Quit(object s, EventArgs e)
        {
            await ButtonClicked(s as Button);
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