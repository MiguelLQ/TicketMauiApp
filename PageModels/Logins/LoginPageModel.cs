using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using MauiFirebase.Pages.Login;
using MauiFirebase.Pages.Register;
using Microsoft.Maui.Networking;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Pages.RecuperarContrasena;


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
                // Validar conexión a internet real
                if (!await HayInternetRealAsync())
                {
                    ErrorMessage = "Necesitas conexión a internet.";
                    HasError = true;
                    return;
                }

                // Validar campos
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Debes ingresar tu correo y contraseña.";
                    HasError = true;
                    return;
                }

                // Intentar login
                var success = await _authService.LoginAsync(Email, Password);
                if (!success)
                {
                    ErrorMessage = "Correo o contraseña incorrectos.";
                    HasError = true;
                    return;
                }

                // ✅ Validar si el correo fue verificado
                var estaVerificado = await _authService.VerificarCorreoElectronicoAsync();
                if (!estaVerificado)
                {
                    ErrorMessage = "Debes verificar tu correo electrónico antes de continuar.";
                    HasError = true;
                    await _authService.Logout(); // cerrar sesión inmediata
                    return;
                }

                // Obtener ID token y UID
                var token = await _authService.ObtenerIdTokenSeguroAsync();
                var uid = Preferences.Get("FirebaseUserId", string.Empty);

                // Consultar datos del usuario desde Firestore
                var usuarioService = new FirebaseUbicacionServie();
                var usuario = await usuarioService.ObtenerUsuarioPorUidAsync(uid, token);

                if (usuario == null)
                {
                    usuario = new Models.Usuario
                    {
                        Uid = uid,
                        Rol = "Ciudadano",
                        Estado = true
                    };
                }

                // Verificar si está activo
                if (!usuario.Estado)
                {
                    ErrorMessage = "Tu cuenta está inactiva. Contáctate con el administrador.";
                    HasError = true;
                    await _authService.Logout();
                    return;
                }

                // Guardar preferencias
                Preferences.Set("FirebaseUserRole", usuario.Rol);
                Preferences.Set("FirebaseUid", usuario.Uid);
                Preferences.Set("FirebaseUserNombre", usuario.Nombre ?? "");
                Preferences.Set("FirebaseUserApellido", usuario.Apellido ?? "");

                // Redirigir según rol
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
                    else if (usuario.Rol == "Conductor")
                        await Shell.Current.GoToAsync("//conductorHome/inicioConductor");
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

        [RelayCommand]
        private async Task IrARecuperarContrasenaAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new RecuperarContrasenaPage());
        }


    }
}
