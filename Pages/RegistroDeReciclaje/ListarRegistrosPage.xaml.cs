using MauiFirebase.PageModels.RegistroDeReciclajes;

namespace MauiFirebase.Pages.RegistroDeReciclaje;

public partial class ListarRegistrosPage : ContentPage
{
    private readonly ListarRegistrosPageModel _pageModel;

    public ListarRegistrosPage(ListarRegistrosPageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _pageModel.CargarResiduoAsync();
        await _pageModel.CargarResidentesAsync();
        await _pageModel.CargarRegistroResiduoAsync();
    }

    private async void OnRegistrarReciclajeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AgregarRegistroPage));
    }
}
