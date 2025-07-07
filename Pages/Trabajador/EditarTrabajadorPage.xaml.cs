using MauiFirebase.PageModels.Trabajadores;

namespace MauiFirebase.Pages.Trabajador;

public partial class EditarTrabajadorPage : ContentPage
{
    private readonly EditarTrabajadorPageModel _viewModel;
    public EditarTrabajadorPage(EditarTrabajadorPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
