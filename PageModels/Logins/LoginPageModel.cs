using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Pages.Login;
using MauiFirebase.Pages.Register;
using MauiFirebase.Pages.Ticket;

namespace MauiFirebase.PageModels.Logins
{
    public partial class LoginPageModel: INotifyPropertyChanged
    {
        private readonly FirebaseAuthService _authService = new FirebaseAuthService();

        private string email;
        private string password;
        private string errorMessage;
        private bool hasError;

        public string Email
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public bool HasError
        {
            get => hasError;
            set => SetProperty(ref hasError, value);
        }

        public ICommand LoginCommand { get; }

        public LoginPageModel()
        {
            LoginCommand = new AsyncRelayCommand(LoginAsync);
        }

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
            // ✅ Navegación directa sin Shell
            await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }



        // Implementación de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value)) return false;
            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }


    }
}
