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
        await _pageModel.LoadRegistrosAsync();
    }

    private async void OnBuscarYAgregarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(BuscarResidentePage));
    }
}
