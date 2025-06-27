using MauiFirebase.PageModels.Premios;

namespace MauiFirebase.Pages.Premio;

public partial class ListarPremioPage : ContentPage
{
    private readonly PremioPageModel _viewModel;

    public ListarPremioPage(PremioPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CargarPremiosAsync(); // ?? Esto actualiza la lista cada vez que entras
    }
}

