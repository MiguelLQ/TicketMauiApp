using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using MauiFirebase.Pages.Login;
using MauiFirebase.Pages.Register;
using Microsoft.Maui.Networking;

namespace MauiFirebase.PageModels.Logins
{
    public partial class LoginPageModel : ObservableObject
    {
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private bool hasError;

        public LoginPageModel()
        {
            LoginCommand = new AsyncRelayCommand(LoginAsync);
        }

        public ICommand LoginCommand { get; }

        private async Task LoginAsync()
        {
            HasError = false;
            ErrorMessage = string.Empty;

            // ✅ 1. Validar conexión a Internet
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                ErrorMessage = "No hay conexión a Internet.";
                HasError = true;
                return;
            }

            // ✅ 2. Validar campos
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Debes ingresar tus datos.";
                HasError = true;
                return;
            }

            // ✅ 3. Intentar login
            var success = await _authService.LoginAsync(Email, Password);
            if (success)
            {
                // Pantalla de carga
                Application.Current.MainPage = new LoadingPage();
                await Task.Delay(500);

                // AppShell
                Application.Current.MainPage = new AppShell();
                // Espera a que AppShell se termine de cargar
                await Task.Delay(200);
                ((AppShell)Application.Current.MainPage).MostrarOpcionesSegunRol(); // Asegúrate de mostrar menú según rol

                var rol = Preferences.Get("FirebaseUserRole", string.Empty);
                System.Diagnostics.Debug.WriteLine($"🟡 ROL desde LoginPageModel: {rol}");


                Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                {
                    if (rol == "admin")
                        await Shell.Current.GoToAsync("//adminHome/inicio");
                    else if (rol == "register")
                        await Shell.Current.GoToAsync("//registerHome/inicio");
                    else
                        await Shell.Current.GoToAsync("//ciudadanoHome/inicioCiudadano");
                });
            }
            else
            {
                ErrorMessage = "Correo o contraseña incorrectos.";
                HasError = true;
            }
        }


        [RelayCommand]
        private async Task IrARegistroAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }
    }
}
