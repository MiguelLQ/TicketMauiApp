using MauiFirebase.PageModels.Logins;

namespace MauiFirebase.Pages.Login;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent(); // ← Esto debe funcionar si el XAML está bien vinculado
        BindingContext = new LoginPageModel();
    }

    private async void RegisterTapped(object sender, EventArgs e)
    {
        await DisplayAlert("Registro", "Navegar a la página de registro", "OK");
    }

    private async void FacebookClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Facebook", "Iniciar sesión con Facebook", "OK");
    }

    private async void GoogleClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Google", "Iniciar sesión con Google", "OK");
    }
}
