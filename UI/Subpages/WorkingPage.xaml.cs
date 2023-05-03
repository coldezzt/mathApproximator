using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Storage;

namespace UI;

public partial class WorkingPage : ContentPage
{
	// TODO ���� ������ � ������ �� ����� �������,
	// ������ �� ������� ��� ������������ ����������
	static string configString = "";

	List<string> possibleExtensions = new List<string>();
	bool hasUnsavedChanges = false;
	Action DataChanged;

	public WorkingPage()
	{
		InitializeComponent();
		DataChanged = () =>
		{
			saveButton.Text = "���������";
			hasUnsavedChanges = true;
		};
		possibleExtensions = _getPossibleExtensions();

		_checkConfiguration();
	}

	public async void ToPreviousPage(object sender, EventArgs e)
	{
		var accepted = true;
		if (hasUnsavedChanges)
		{
			accepted = await DisplayAlert("����������� ��������", "� ��� ���� ������������ ���������. �� ������� ��� ������ ���������?", "��", "���");
			if (!accepted) return;
		}

        await Navigation.PopAsync();
    }

    public async void GetPathToDataFromFilePicker(object sender, EventArgs e)
	{
        string result = null;
		try
		{
			result = (await FilePicker.PickAsync(default)).FullPath;
		}
		catch 
		{
			return;
		}

		dataPathEntry.Text = result;
		DataChanged();
	}

	public void GetPathToDataFromEntryField(object sender, EventArgs e)
	{
		if (sender is Entry entry)
		{
			if (entry.Text == "" || entry.Text == null)
			{ }
			else if (!Path.Exists(entry.Text))
			{
				dataPathWarnLabel.Opacity = 1;
				dataPathWarnLabel.Text = "���� �� ������";
			}
			else if (!possibleExtensions.Contains(Path.GetExtension(entry.Text)))
			{
				dataPathWarnLabel.Opacity = 1;
				dataPathWarnLabel.Text = "���������������� ���������� �����";
			}
			else
			{
				dataPathWarnLabel.Opacity = 0;
			}
			DataChanged();
		}
	}

	public async void PathToResults(object sender, EventArgs e)
	{
		/* 
		 * ��� ���������� ������ ����� ������������ ��� �����������
		var str = $"{dataPathEntry.Text};{Math.Round(dataAccuracy.Value)}";
		using (var sw = new MemoryStream(Encoding.Default.GetBytes(str)))
		{
			var resultPath = await FileSaver.SaveAsync(@"C:\\", "config.txt", sw, cts.Token);
		}
		*/
		string path = string.Empty;
		if (sender is Button)
		{
			try
			{
				var folderResult = await FolderPicker.PickAsync(default);
			
				folderResult.Deconstruct(out var folder, out _);
				path = folder.Path;
			}
			catch { }
		}
		if (sender is Entry entry)
			path = entry.Text;
		
		dataResultsPath.Text = path;
		DataChanged();
    }

    public void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
		accuracyHeader.Text = $"�������� (����� ����� �������): {Math.Round(e.NewValue)}";
        DataChanged();
    }

    public async void StartAlgorithm(object sender, EventArgs e)
	{
		if (dataPathWarnLabel.Opacity == 0 
			&& dataPathEntry.Text != string.Empty
			&& dataPathEntry.Text != null)
		{
			SaveAlgorithmSettings(new object(), new EventArgs());
			await Navigation.PushAsync(
                new AlgorithmPage(dataPathEntry.Text,
                                  dataResultsPath.Text,
								  (int)Math.Round(dataAccuracy.Value)
								  ));
		}
		else
			await DisplayAlert("�������� ������", "���� ��� ��������� ����� ��������� �������, ��������� ������������ �������� ������ � ���������� ��� ���", "�����");
	}

	public async void SaveAlgorithmSettings(object sender, EventArgs e)
    {
		/*
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		
		using (var sw = new StreamWriter(Path.Combine(path, "algorithmConfig.cfg")))
		{
			sw.Write($"{dataPathEntry.Text};{dataResultsPath.Text};{Math.Round(dataAccuracy.Value)}");
		}
		*/
		configString = $"{dataPathEntry.Text};{dataResultsPath.Text};{Math.Round(dataAccuracy.Value)}";

		hasUnsavedChanges = false;

		if (sender is Button btn)
		{
			btn.Text = "���������!";
            await btn.BackgroundColorTo(Color.FromArgb("0fd44a"), 1, 100);
            await btn.BackgroundColorTo(Color.FromArgb("FFFFFF"), 1, 3000);
		}
    }

    private void _checkConfiguration()
    {
        /*
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (Path.Exists(Path.Combine(path, "algorithmConfig.cfg")))
		{
			
			string[] config = File.ReadAllText(path).Split(";");
			dataPathEntry.Text = config[0];
			dataResultsPath.Text = config[1];
			dataAccuracy.Value = int.Parse(config[2]);
		}
		*/

        if (configString != string.Empty)
        {
            string[] config = configString.Split(';');
            dataPathEntry.Text = config[0];
            dataResultsPath.Text = config[1];
            dataAccuracy.Value = int.Parse(config[2]);
        }
    }

	private List<string> _getPossibleExtensions()
    {
		// TODO ��������� ��������� ����������
        return new List<string>() { ".txt" };
    }


}