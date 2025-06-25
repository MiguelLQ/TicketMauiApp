using MauiFirebase.PageModels.Canjes;

namespace MauiFirebase.Pages.Canje;

public partial class EditarCanjePage : ContentPage
{
    private readonly EditarCanjePageModel _viewModel;

    public EditarCanjePage(EditarCanjePageModel viewModel)
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
