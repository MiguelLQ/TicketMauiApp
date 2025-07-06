using MauiFirebase.PageModels.Vehiculos;

namespace MauiFirebase.Pages.Vehiculo;

public partial class EditarVehiculoPage : ContentPage
{
    private readonly EditarVehiculoPageModel _viewModel;
    public EditarVehiculoPage(EditarVehiculoPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
