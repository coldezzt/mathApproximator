using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Storage;
using System.Reflection;

namespace UI;

public partial class WorkingPage : ContentPage
{
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


	public async void ToPreviousPage(object s, EventArgs e)
	{
		var accepted = true;
		if (hasUnsavedChanges)
		{
			accepted = await DisplayAlert("����������� ��������", "� ��� ���� ������������ ���������. �� ������� ��� ������ ���������?", "��", "���");
			if (!accepted) return;
		}

        await Navigation.PopAsync();
    }

    public async void GetDataPathFromFilePicker(object s, EventArgs e)
	{
		string result = null;
		try
		{
			result = (await FilePicker.PickAsync(default)).FullPath;
		}
		catch { }

		dataPathEntry.Text = result;
		DataChanged();
	}

	public void GetDataPathFromEntryField(object s, EventArgs e)
	{
		if (s is Entry entry)
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

	public async void PathToResults(object s, EventArgs e)
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
		if (s is Button)
		{
			try
			{
				var folderResult = await FolderPicker.PickAsync(default);
			
				folderResult.Deconstruct(out var folder, out _);
				path = folder.Path;
			}
			catch { }
		}
		if (s is Entry entry)
			path = entry.Text;
		
		dataResultsPath.Text = path;
		DataChanged();
    }

    public void OnSliderValueChanged(object s, ValueChangedEventArgs e)
    {
		accuracyHeader.Text = $"�������� (����� ����� �������): {Math.Round(e.NewValue)}";
        DataChanged();
    }

    public async void StartAlgorithm(object s, EventArgs e)
	{
		await DisplayAlert("���� ����������?", "��� ������ ��� ����)", "����� �������(");
	}

	public async void SaveAlgorithmSettings(object s, EventArgs e)
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

		if (s is Button btn)
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

    private async Task<int> _buttonClicked(Button btn)
    {
        await btn.BackgroundColorTo(Color.FromArgb("666666"), 1, 100);
        await btn.BackgroundColorTo(Color.FromArgb("FFFFFF"), 1, 100);
		btn.BackgroundColor = Color.FromArgb("FFFFFF");
        return 0;
    }

	private List<string> _getPossibleExtensions()
    {
		// TODO ��������� ��������� ����������
        return new List<string>() { ".txt" };
    }


}