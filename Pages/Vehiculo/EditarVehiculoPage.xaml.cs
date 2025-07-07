using MauiFirebase.PageModels.Vehiculos;

namespace MauiFirebase.Pages.Vehiculo;

public partial class EditarVehiculoPage : ContentPage
{
    private readonly EditarVehiculoPageModel _viewModel;
    public EditarVehiculoPage(EditarVehiculoPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EditarVehiculoPageModel vm)
        {
            await vm.InicializarAsync();
        }
    }
}
