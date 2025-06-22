using MauiFirebase.PageModels.Canjes;

namespace MauiFirebase.Pages.Canje;

public partial class AgregarCanjePage : ContentPage
{
    private readonly CanjePageModel _pageModel;

    public AgregarCanjePage(CanjePageModel pageModel)
    {
        InitializeComponent();
        _pageModel = pageModel;
        BindingContext = _pageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _pageModel.LoadAsync();         // Cargar lista de canjes
        await _pageModel.CargarPremioAsync(); // Cargar lista de premios
    }
}
