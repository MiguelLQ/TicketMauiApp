using MauiFirebase.PageModels.Canjes;

namespace MauiFirebase.Pages.Canje;

public partial class AgregarCanjePage : ContentPage
{
    private readonly CrearCanjePageModel _viewModel;

    public AgregarCanjePage(CrearCanjePageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarPremiosAsync();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
