using MauiFirebase.PageModels.Usuarios;
using Syncfusion.Maui.Toolkit.Carousel;

namespace MauiFirebase.Pages.usuario;

public partial class EditarUsuarioPage : ContentPage
{
    private readonly UsuarioPageModel _viewModel;

    public EditarUsuarioPage(UsuarioPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}