using MauiFirebase.PageModels.Logins;

namespace MauiFirebase.Pages.Home;

public partial class InicioPage : ContentPage
{
    private readonly DashboardPageModel _viewModel;
	public InicioPage(DashboardPageModel viewModel)
	{
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetBackgroundColor(this, Color.FromArgb("#3949ab"));                                                      
        Shell.SetForegroundColor(this, Colors.White); 
        await _viewModel.InicializarAsync();  
        MostrarCorreoUsuario();  
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