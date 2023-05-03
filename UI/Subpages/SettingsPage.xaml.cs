namespace UI;

public partial class SettingsPage : ContentPage
{
	// —делать выбор пути к библеотеке в первый раз когда приложение открыто
	// ѕоследующие разы не вызывать и мен€ть только в настройках
	// возможно добавить множество библеотек и их выбор
	static public string LibraryPath = "";

	public SettingsPage()
	{
		InitializeComponent();
	}

	public async void ToPreviousPage(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}

    public async void ToInstructionsPage(object sender, EventArgs e)
    {
		await Navigation.PushAsync(new InstructionsPage());
    }
}