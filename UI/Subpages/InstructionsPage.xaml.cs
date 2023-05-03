namespace UI;

public partial class InstructionsPage : ContentPage
{
	public InstructionsPage()
	{
		InitializeComponent();
	}

	async public void ToPreviousPage(object s, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}