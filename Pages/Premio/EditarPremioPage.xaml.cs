using MauiFirebase.PageModels.Premios;

namespace MauiFirebase.Pages.Premio;

public partial class EditarPremioPage : ContentPage
{
    private readonly EditarPremioPageModel _viewModel;

    public EditarPremioPage(EditarPremioPageModel viewModel)
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
        await Shell.Current.GoToAsync("..");
    }
}