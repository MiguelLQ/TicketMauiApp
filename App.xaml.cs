using MauiFirebase.Pages.Login;

namespace MauiFirebase
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitializeAppAsync(); // Inicializar la aplicación de forma asíncrona
        }
        private async void InitializeAppAsync()
        {
            var authService = new FirebaseAuthService();
            if (authService.IsLoggedIn())
            {
                await authService.ObtenerIdTokenSeguroAsync();
                MainPage = new AppShell(); // ir directo al sistema
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage()); // mostrar login

            }
        }
    }
}