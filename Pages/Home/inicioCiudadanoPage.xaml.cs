using MauiFirebase.Data.Interfaces;
using MauiFirebase.PageModels.Residentes;
using Syncfusion.Maui.Toolkit.Carousel;

namespace MauiFirebase.Pages.Home;

public partial class inicioCiudadanoPage : ContentPage
{
	
    private readonly InicioCiudadanoPageModel _viewModel;

    public inicioCiudadanoPage(InicioCiudadanoPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetBackgroundColor(this, Color.FromArgb("#5061c8"));
        Shell.SetForegroundColor(this, Colors.White);
        await _viewModel.CargarDatosUsuarioAsync();
    }
}