using MauiFirebase.PageModels.Logins;

namespace MauiFirebase.Pages.Login;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
        BindingContext = new LoginPageModel(); // Usa el ViewModel correctamente
    }

    private async void FacebookClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Facebook", "Iniciar sesión con Facebook", "OK");
    }

    
}
