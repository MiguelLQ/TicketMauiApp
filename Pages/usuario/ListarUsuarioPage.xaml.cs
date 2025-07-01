using MauiFirebase.PageModels.Usuarios;

namespace MauiFirebase.Pages.usuario;

public partial class ListarUsuarioPage : ContentPage
{
	public ListarUsuarioPage()
	{
		InitializeComponent();
        BindingContext = new UsuarioPageModel();
    }
}