using MauiFirebase.PageModels.Usuarios;

namespace MauiFirebase.Pages.usuario;

public partial class ListarUsuarioPage : ContentPage
{
    private readonly UsuarioPageModel _viewModel;
    public ListarUsuarioPage( UsuarioPageModel viewModel)
	{
		InitializeComponent();
        BindingContext = _viewModel = viewModel;
        VerificarPermiso();
    }
    public async void OnAgregarUsuarioClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///usuarios/agregar");
    }

    private async void OnEditarUsuarioClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.CommandParameter is Models.Usuario usuario)
        {
            _viewModel.UsuarioNuevo = usuario;
            await Shell.Current.GoToAsync("///usuarios/editar"); // Navegación absoluta
        }
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is UsuarioPageModel vm)
            _ = vm.CargarUsuariosAsync(); // vuelve a recargar al entrar
    }


    private async void VerificarPermiso()
    {
        var rol = Preferences.Get("FirebaseUserRole", string.Empty);

        if (rol != "admin")
        {
            await DisplayAlert("Acceso denegado", "No tienes permiso para acceder a esta página.", "OK");
            await Shell.Current.GoToAsync(".."); // O navega al home
        }
    }
}