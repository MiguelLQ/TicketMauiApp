using MauiFirebase.PageModels.Rutas;

namespace MauiFirebase.Pages.Ruta;

public partial class ListarRutaPage : ContentPage
{
    private readonly RutaPageModel _pageModel;

    public ListarRutaPage(RutaPageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _pageModel.CargarRutasAsync();
    }

    private async void OnAgregarRutaClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AgregarRutaPage));
    }

    private async void OnEditarRutaClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is MauiFirebase.Models.Ruta ruta)
        {
            await Shell.Current.GoToAsync($"{nameof(EditarRutaPage)}?id={ruta.IdRuta}");
        }
    }
}
