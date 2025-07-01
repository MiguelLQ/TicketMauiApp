using MauiFirebase.PageModels.Logins;
using MauiFirebase.Services;

namespace MauiFirebase.Pages.Home;

public partial class InicioPage : ContentPage
{

	public InicioPage()
	{
		InitializeComponent();
        BindingContext = new DashboardPageModel();
        MostrarCorreoUsuario();

    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        Shell.SetBackgroundColor(this, Color.FromArgb("#3949ab")); // Tu color morado personalizado
        // Shell.SetTitleColor(this, Colors.White); Color del título
        Shell.SetForegroundColor(this, Colors.White); // Iconos y botones
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        Shell.SetBackgroundColor(this, Application.Current.RequestedTheme == AppTheme.Dark ? Colors.Black : Colors.White);
        Shell.SetTitleColor(this, Colors.Black);
        Shell.SetForegroundColor(this, Colors.Black);
    }
    private void MostrarCorreoUsuario()
    {
        var authService = new FirebaseAuthService();
        string correo = authService.GetUserEmail();
        CorreoUsuarioLabel.Text = correo;
    }

}