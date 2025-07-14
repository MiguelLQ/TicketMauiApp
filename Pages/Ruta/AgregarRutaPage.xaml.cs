using MauiFirebase.PageModels.Rutas;

namespace MauiFirebase.Pages.Ruta;

public partial class AgregarRutaPage : ContentPage
{
    private readonly CrearRutaPageModel _viewModel;

    public AgregarRutaPage(CrearRutaPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarVehiculosAsync();
    }
}
