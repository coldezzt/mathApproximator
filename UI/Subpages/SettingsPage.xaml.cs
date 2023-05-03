namespace UI;

public partial class SettingsPage : ContentPage
{
	// ������� ����� ���� � ���������� � ������ ��� ����� ���������� �������
	// ����������� ���� �� �������� � ������ ������ � ����������
	// �������� �������� ��������� ��������� � �� �����
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