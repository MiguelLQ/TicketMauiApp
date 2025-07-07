using MauiFirebase.PageModels.Vehiculos;

namespace MauiFirebase.Pages.Vehiculo;

public partial class AgregarVehiculoPage : ContentPage
{
    private readonly CrearVehiculoPageModel _viewModel;
    public AgregarVehiculoPage(CrearVehiculoPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarUsuariosAsync();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
