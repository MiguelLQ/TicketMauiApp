using MauiFirebase.PageModels.Premios;

namespace MauiFirebase.Pages.Premio;

public partial class ListarPremioPage : ContentPage
{
    private readonly PremioPageModel _viewModel;

    public ListarPremioPage(PremioPageModel viewModel)
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
}
