namespace UI;

public partial class InstructionsPage : ContentPage
{
	public InstructionsPage()
	{
		InitializeComponent();
	}

	async public void ToPreviousPage(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}