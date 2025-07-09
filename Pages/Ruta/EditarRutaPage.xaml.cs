using MauiFirebase.PageModels.Rutas;

namespace MauiFirebase.Pages.Ruta;

public partial class EditarRutaPage : ContentPage
{
    private readonly EditarRutaPageModel _viewModel;

    public EditarRutaPage(EditarRutaPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InicializarAsync();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
