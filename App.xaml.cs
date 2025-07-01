using MauiFirebase.Pages.Login;

namespace MauiFirebase
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var authService = new FirebaseAuthService();

            if (authService.IsLoggedIn())
            {
                MainPage = new AppShell(); // ir directo al sistema
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());// mostrar login
            }
        }

    }
}