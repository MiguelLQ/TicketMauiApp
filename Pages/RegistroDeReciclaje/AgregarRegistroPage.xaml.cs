using MauiFirebase.PageModels.RegistroDeReciclajes;

namespace MauiFirebase.Pages.RegistroDeReciclaje;

public partial class AgregarRegistroPage : ContentPage
{
    private readonly AgregarRegistroPageModel _viewModel;

    public AgregarRegistroPage(AgregarRegistroPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarResiduoAsync();
        await _viewModel.CargarRegistroReciclajeAsync();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
