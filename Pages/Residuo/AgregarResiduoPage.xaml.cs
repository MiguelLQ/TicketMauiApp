using MauiFirebase.PageModels.Residuos;

namespace MauiFirebase.Pages.Residuo;

public partial class AgregarResiduoPage : ContentPage
{
    private readonly ResiduoPageModel _viewModel;

    public AgregarResiduoPage(ResiduoPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarResiduosAsync();
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}