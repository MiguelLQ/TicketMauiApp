using MauiFirebase.PageModels.Vehiculos;
using MauiFirebase.Models;

namespace MauiFirebase.Pages.Vehiculo;

public partial class ListarVehiculoPage : ContentPage
{
    private readonly VehiculoPageModel _pageModel;
    public ListarVehiculoPage(VehiculoPageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _pageModel.CargarVehiculosAsync();
    }

    private async void OnAgregarVehiculoClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("AgregarVehiculoPage");
    }

    private async void OnEditarVehiculoClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is MauiFirebase.Models.Vehiculo vehiculo)
        {
            await Shell.Current.GoToAsync($"EditarVehiculoPage?id={vehiculo.IdVehiculo}");
        }
    }

    private async void OnCambiarEstadoVehiculoClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is int id)
        {
            await _pageModel.CambiarEstadoVehiculoAsync(id);
        }
    }
}
