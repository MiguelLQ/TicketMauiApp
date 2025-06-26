using MauiFirebase.PageModels.Logins;

namespace MauiFirebase.Pages.Login;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
        BindingContext = new LoginPageModel();

    }
}