using MauiFirebase.PageModels.CategoriaResiduos;

namespace MauiFirebase.Pages.CategoriaResiduo;

public partial class CategoriaResiduoPage : ContentPage
{
    private readonly CategoriaResiduoPageModel _viewModel;

    public CategoriaResiduoPage(CategoriaResiduoPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // Asegurarse de que ambas cargas ocurran tambi�n aqu� por si el constructor ya pas� y la vista se recarga.
            await _viewModel.CargarTicketsAsync();
            await _viewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo cargar la informaci�n de ticket: {ex.Message}", "OK");
        }
    }
}