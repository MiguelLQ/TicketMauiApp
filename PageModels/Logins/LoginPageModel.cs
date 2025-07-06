using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using MauiFirebase.Pages.Login;
using MauiFirebase.Pages.Register;

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

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Debes ingresar tus datos.";
                HasError = true;
                return;
            }

            var success = await _authService.LoginAsync(Email, Password);
            if (success)
            {
                // Mostrar pantalla de carga temporal
                Application.Current.MainPage = new LoadingPage();
                await Task.Delay(500);

                // Establecer el Shell
                Application.Current.MainPage = new AppShell();

                // Leer el rol desde Preferences
                var rol = Preferences.Get("FirebaseUserRole", string.Empty);

                // Navegar según el rol
                Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                {
                    if (rol == "admin")
                        await Shell.Current.GoToAsync("//adminHome/inicio");
                    else if (rol == "register")
                        await Shell.Current.GoToAsync("//registerHome/inicio");
                    else
                        await Shell.Current.DisplayAlert("Rol desconocido", "No se pudo determinar tu panel de acceso.", "OK");
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
