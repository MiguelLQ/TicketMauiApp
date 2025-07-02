using MauiFirebase.PageModels.Usuarios;

namespace MauiFirebase.Pages.usuario;

public partial class AgregarUsuarioPage : ContentPage
{
	private readonly UsuarioPageModel _viewModel;
    public AgregarUsuarioPage(UsuarioPageModel viewModel)
	{
		InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }
    private async void OnVolverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

}