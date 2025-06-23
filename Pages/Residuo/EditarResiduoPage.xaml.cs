using MauiFirebase.PageModels.Residuos;

namespace MauiFirebase.Pages.Residuo;

public partial class EditarResiduoPage : ContentPage
{
    private readonly EditarResiduoPageModel _viewModel;

    public EditarResiduoPage(EditarResiduoPageModel viewModel)
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