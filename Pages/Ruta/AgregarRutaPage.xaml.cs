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

    private async void OnCrearRutaNuevaClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new DibujarRutaPage(_viewModel));
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
