using Syncfusion.Maui.Toolkit.Carousel;

namespace MauiFirebase.Pages.Home;

public partial class inicioCiudadanoPage : ContentPage
{
	public inicioCiudadanoPage()
	{
		InitializeComponent();
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetBackgroundColor(this, Color.FromArgb("#5061c8"));
        Shell.SetForegroundColor(this, Colors.White);
        
    }
}