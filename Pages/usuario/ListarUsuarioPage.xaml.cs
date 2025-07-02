using MauiFirebase.PageModels.Usuarios;

namespace MauiFirebase.Pages.usuario;

public partial class ListarUsuarioPage : ContentPage
{
    private readonly UsuarioPageModel _viewModel;
    public ListarUsuarioPage( UsuarioPageModel viewModel)
	{
		InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }
    public async void OnAgregarUsuarioClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///usuarios/agregar");
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is UsuarioPageModel vm)
            _ = vm.CargarUsuariosAsync(); // vuelve a recargar al entrar
    }



}