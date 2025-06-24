using MauiFirebase.PageModels.Premios;

namespace MauiFirebase.Pages.Premio;

public partial class AgregarPremioPage : ContentPage
{
    private readonly CrearPremioPageModel _viewModel;

    public AgregarPremioPage(CrearPremioPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}