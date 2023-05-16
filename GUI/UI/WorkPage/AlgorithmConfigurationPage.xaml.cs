using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Storage;

namespace UI;

public partial class AlgorithmConfigurationPage : ContentPage
{
	private readonly List<string> possibleExtensions = new();
	private readonly Action DataChanged;
	private bool hasUnsavedChanges = false;

	public AlgorithmConfigurationPage()
	{
		InitializeComponent();
		DataChanged = () =>
		{
			saveButton.Text = "Сохранить";
			hasUnsavedChanges = true;
		};
		possibleExtensions = new List<string>() { ".txt" };
	}

    public async void ToPreviousPage(object sender, EventArgs e)
	{
		bool accepted;
		if (hasUnsavedChanges)
		{
			accepted = await DisplayAlert("Подтвердить действие", "У Вас есть несохранённые параметры. Вы уверены что хотите вернуться?", "Да", "Нет");
			if (!accepted) return;
		}

        await Navigation.PopAsync();
    }
    public async void GetPathToDataFromFilePicker(object sender, EventArgs e)
	{
		try
		{
			var result = (await FilePicker.PickAsync(default)).FullPath;
			dataPathEntry.Text = result;
            DataChanged();
        }
		catch 
		{
			return;
		}
	}
    public void GetPathToDataFromEntryField(object sender, EventArgs e)
    {
        if (sender is Entry entry)
        {
            if (entry.Text == "" || entry.Text == null)
                dataPathWarnLabel.Opacity = 0;

            else if (!Path.Exists(entry.Text))
            {
                dataPathWarnLabel.Opacity = 1;
                dataPathWarnLabel.Text = "Файл не найден";
            }
            else if (!possibleExtensions.Contains(Path.GetExtension(entry.Text)))
            {
                dataPathWarnLabel.Opacity = 1;
                dataPathWarnLabel.Text = "Необрабатываемое расширение файла";
            }
            else
            {
                dataPathWarnLabel.Opacity = 0;
            }
            DataChanged();
        }
    }
    public async void GetPathToLibraryFromFilePicker(object sender, EventArgs e)
    {
        try
        {
            var result = (await FilePicker.PickAsync(default)).FullPath;
            libraryPathEntry.Text = result;
            DataChanged();
        }
        catch
        {
            return;
        }
    }
    public void GetPathToLibraryFromEntryField(object sender, EventArgs e)
	{
		if (sender is Entry entry)
		{
			if (entry.Text == "" || entry.Text == null)
				libraryPathWarnLabel.Opacity = 0;

			else if (!Path.Exists(entry.Text))
			{
				libraryPathWarnLabel.Opacity = 1;
				libraryPathWarnLabel.Text = "Файл не найден";
			}
			else if (Path.GetExtension(entry.Text) != ".dll")
			{
				libraryPathWarnLabel.Opacity = 1;
				libraryPathWarnLabel.Text = "Неверное расширение файла";
			}
			else
			{
				libraryPathWarnLabel.Opacity = 0;
			}
			DataChanged();
		}
	}
	public async void ChangePathToResults(object sender, EventArgs e)
	{
		string path = string.Empty;
		FolderPickerResult folderResult = null;
		if (sender is Button)
		{
			try
			{
				folderResult = await FolderPicker.PickAsync(default);
			}
			catch { }

			folderResult.Deconstruct(out var folder, out _);
            if (folder != null)
			{
				path = folder.Path;
			}
        }

		if (sender is Entry entry)
			path = entry.Text;
		
		dataResultsPath.Text = path;
		DataChanged();
    }
    public void ChangeAlgorithmAccuracy(object sender, ValueChangedEventArgs e)
    {
		accuracyHeader.Text = $"Точность (знаки после запятой): {Math.Round(e.NewValue)}";
        DataChanged();
    }
    public async void StartAlgorithm(object sender, EventArgs e)
	{
		if (dataPathWarnLabel.Opacity == 0 
			&& libraryPathWarnLabel.Opacity == 0
			&& dataPathEntry.Text != string.Empty
			&& dataPathEntry.Text != null 
			&& libraryPathEntry.Text != string.Empty
			&& libraryPathWarnLabel.Text != null)
		{
			SaveAlgorithmSettings(new object(), new EventArgs());
			await Navigation.PushAsync(new AlgorithmPage());
		}
		else
			await DisplayAlert("Неверные данные", 
				"Одно или несколько полей заполнено неверно, проверьте корректность " +
				"вводимых данных и попробуйте ещё раз", "Назад");
	}
	public async void SaveAlgorithmSettings(object sender, EventArgs e)
    {
		var dataConfig = Path.GetFileNameWithoutExtension(Settings.PathToAlgorithmData) + "-" + Settings.AlgorithmAccuracy;
		Settings.PathToCalculatingLibrary = libraryPathEntry.Text;
		Settings.PathToAlgorithmData = dataPathEntry.Text;
		Settings.AlgorithmAccuracy = (int)Math.Round(dataAccuracySlider.Value);

		if (dataResultsPath.Text == string.Empty || dataResultsPath.Text == null)
			Settings.PathToAlgorithmResults = 
				Path.Combine(Path.GetDirectoryName(dataPathEntry.Text), $"results{dataConfig}.txt");

		else
			Settings.PathToAlgorithmResults = 
				Path.Combine(Path.GetDirectoryName(dataResultsPath.Text), $"results{dataConfig}.txt");

		hasUnsavedChanges = false;

		if (sender is Button btn)
		{
			var oldBackgroundColor = btn.BackgroundColor;
			btn.Text = "Сохранено!";
            await btn.BackgroundColorTo(Color.FromArgb("0fd44a"), 1, 100);
            await btn.BackgroundColorTo(oldBackgroundColor, 1, 3000);
		}
    }
    
    public void UploadSettings(object sender, EventArgs e)
    {
		libraryPathEntry.Text = Settings.PathToCalculatingLibrary;
        dataPathEntry.Text = Settings.PathToAlgorithmData;
        dataResultsPath.Text = Settings.PathToAlgorithmResults;
        dataAccuracySlider.Value = Settings.AlgorithmAccuracy;
		hasUnsavedChanges = false;
    }
	public void UpdateSettings(object sender, EventArgs e)
	{
		Settings.UpdateConfig();
	}
}