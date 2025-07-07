using MauiFirebase.PageModels.Trabajadores;

namespace MauiFirebase.Pages.Trabajador;

public partial class AgregarTrabajadorPage : ContentPage
{
    private readonly CrearTrabajadorPageModel _viewModel;
    public AgregarTrabajadorPage(CrearTrabajadorPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
