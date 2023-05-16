namespace UI;


public partial class SettingsPage : ContentPage
{
	// —делать выбор пути к библеотеке в первый раз когда приложение открыто
	// ѕоследующие разы не вызывать и мен€ть только в настройках
	// возможно добавить множество библеотек и их выбор

	public SettingsPage()
	{
		InitializeComponent();
	}

	public async void ToPreviousPage(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}

	public void ChangeTheme(object sender, EventArgs e)
	{
		if (sender is CheckBox checkBox)
		{
			if (checkBox.IsChecked)
				Settings.Theme = AppTheme.Dark;

			else
				Settings.Theme = AppTheme.Light;
		}
	}

    // “ут 4 одинаковых метода, как это фиксить € пока не знаю
    public async void ChangePathToLibrary(object sender, EventArgs e)
	{
		var newPath = await CheckEntryField(sender);

		if (IsPathCorrect(newPath, ".dll") == 1)
			Settings.PathToCalculatingLibrary = newPath;

		else
			ChangeWarningLabel(sender, 1);
    }
	public async void ChangePathToGnuplot(object sender, EventArgs e)
	{
		var newPath = await CheckEntryField(sender);

		if (IsPathCorrect(newPath, ".exe") == 1)
			Settings.PathToGnuplot = newPath;

		else
			ChangeWarningLabel(sender, 1);
			
    }
	public async void ChangePathToGnuplotScript(object sender, EventArgs e)
	{
        var newPath = await CheckEntryField(sender);

        if (IsPathCorrect(newPath, ".p") == 1)
            Settings.PathToGnuplotScript = newPath;

		else
			ChangeWarningLabel(sender, 1);
    }
	public async void ChangePathToPhotos(object sender, EventArgs e)
	{
        var newPath = await CheckEntryField(sender);

        if (IsPathCorrect(newPath, "") == 1)
            Settings.PathToSurfaceImages = newPath;

        else
			ChangeWarningLabel(sender, 1);
    }

    public async void ToInstructionsPage(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new InstructionsPage());
    }
	public void UploadSettings(object sender, EventArgs e)
	{
		Settings.UploadConfig();
		themeCheckBox.IsChecked = Settings.Theme == AppTheme.Dark;
		libraryPathEntry.Text = Settings.PathToCalculatingLibrary;
		gnuplotPathEntry.Text = Settings.PathToGnuplot;
		gnuplotScriptPathEntry.Text = Settings.PathToGnuplotScript;
		photosPathEntry.Text = Settings.PathToSurfaceImages;
	}
	public void UpdateSettings(object sender, EventArgs e)
	{
		Settings.UpdateConfig();
	}

	private void ChangeWarningLabel(object sender, int opacity)
	{
        var elem = (sender as Element).Parent.Parent;
        if (sender is Button)
            ((elem as Layout)[2] as Label).Opacity = 1;

        else
            ((elem.Parent as Layout)[2] as Label).Opacity = 1;
    }
	private async Task<string> CheckEntryField(object sender)
	{
		var newPath = "";
		if (sender is Button btn)
		{
			newPath = await GetPathFromFilePicker();
			if (newPath != null)
				(((btn.Parent as Layout)[0] as Frame).Content as Entry).Text = newPath;
		}
		else if (sender is Entry entry)
			newPath = entry.Text;

		return newPath;
    }
	private async Task<string> GetPathFromFilePicker()
	{
		try
		{
			var result = (await FilePicker.PickAsync(default))?.FullPath ?? "";

			return result;
		}
		catch
		{
			return null;
		}
	}
	private int IsPathCorrect(string fullPath, string expectedFileExtension)
	{
		return (fullPath == null || fullPath == "")
			? -1
			: !Path.Exists(fullPath)
				? -1
				: Path.GetExtension(fullPath) != expectedFileExtension
					? 0 : 1;
    }
}