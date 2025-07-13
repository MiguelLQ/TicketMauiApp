using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using MauiFirebase.Pages.Login;
using MauiFirebase.Pages.Register;
using Microsoft.Maui.Networking;
using MauiFirebase.Helpers.Interface;

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

            try
            {
                // ✅ 1. Validar conexión real a Internet
                if (!await HayInternetRealAsync())
                {
                    ErrorMessage = "Necesitas conexion a internet";
                    HasError = true;
                    return;
                }

                // ✅ 2. Validar campos
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Debes ingresar tu correo y contraseña.";
                    HasError = true;
                    return;
                }

                // ✅ 3. Intentar login con Firebase Auth
                var success = await _authService.LoginAsync(Email, Password);
                if (!success)
                {
                    ErrorMessage = "Correo o contraseña incorrectos.";
                    HasError = true;
                    return;
                }

                // ✅ 4. Verificar estado del usuario desde Firestore
                var token = await _authService.ObtenerIdTokenSeguroAsync();
                var uid = Preferences.Get("FirebaseUserId", string.Empty);

                var usuarioService = new FirebaseUsuarioService();
                var usuario = await usuarioService.ObtenerUsuarioPorUidAsync(uid, token);

                if (usuario == null)
                {
                    ErrorMessage = "No se pudo obtener la información del usuario.";
                    HasError = true;
                    return;
                }

                if (!usuario.Estado)
                {
                    ErrorMessage = "Tu cuenta está inactiva. Contáctate con el administrador.";
                    HasError = true;
                    _authService.Logout(); // Limpia sesión
                    return;
                }

                // ✅ 5. Guardar preferencias y navegar según rol
                Preferences.Set("FirebaseUserRole", usuario.Rol);
                Preferences.Set("FirebaseUid", usuario.Uid);

                Application.Current.MainPage = new LoadingPage();
                await Task.Delay(500);

                Application.Current.MainPage = new AppShell();
                await Task.Delay(200);
                ((AppShell)Application.Current.MainPage).MostrarOpcionesSegunRol();

                Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                {
                    if (usuario.Rol == "Administrador")
                        await Shell.Current.GoToAsync("//adminHome/inicio");
                    else if (usuario.Rol == "Recolector")
                        await Shell.Current.GoToAsync("//registerHome/inicio");
                    else
                        await Shell.Current.GoToAsync("//ciudadanoHome/inicioCiudadano");
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en LoginAsync: " + ex.Message);
                ErrorMessage = "Ocurrió un error inesperado. Intenta nuevamente.";
                HasError = true;
            }
        }
        private async Task<bool> HayInternetRealAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                var result = await client.GetAsync("https://www.google.com");
                return result.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        [RelayCommand]
        private async Task IrARegistroAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }
    }
}
