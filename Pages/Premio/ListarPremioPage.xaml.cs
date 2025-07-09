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

        _viewModel.VerificarRol(); // Esto carga EsAdmin en base al rol guardado
        await _viewModel.CargarPremiosAsync();
    }

}

