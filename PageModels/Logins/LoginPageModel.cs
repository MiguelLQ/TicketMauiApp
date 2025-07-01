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
                // Mostrar pantalla de carga
                Application.Current.MainPage = new LoadingPage();

                // Pequeño delay opcional para mostrar el loading
                await Task.Delay(500);

                // Cargar el shell
                Application.Current.MainPage = new AppShell();

                Application.Current.MainPage.Dispatcher.Dispatch(async () =>
                {
                    await Shell.Current.GoToAsync("//Home/inicio");
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
