using System.Diagnostics;
using System.Reflection;

namespace UI;

public partial class AlgorithmPage : ContentPage
{
	public AlgorithmPage(string dataPath, string dataResult, int dataAccuracy)
	{
		InitializeComponent();

		Calculate(dataPath, dataResult, dataAccuracy);
	}

	public async void ToPreviousPage(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}

	public async void Calculate(string dataPath, string dataResult, int dataAccuracy)
	{
		await new Task(() => UploadLibrary(SettingsPage.LibraryPath));
    }

    public async void Smth(object sender, EventArgs e)
    {
        await new Task(() => UploadLibrary(SettingsPage.LibraryPath));
    }

    private void UploadLibrary(string libraryPath)
	{
		Assembly assembly = Assembly.LoadFrom(libraryPath);

		Type[] approximatorType = assembly.GetTypes();

		//TODO Implement
	}
}