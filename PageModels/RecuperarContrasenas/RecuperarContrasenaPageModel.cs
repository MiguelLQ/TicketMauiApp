using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace MauiFirebase.PageModels.RecuperarContrasenas

{
    public partial class RecuperarContrasenaPageModel : ObservableObject
    {
        private readonly FirebaseAuthService _authService = new();

        [ObservableProperty]
        private string? email;

        [ObservableProperty]
        private string? mensaje;

        [RelayCommand]
        private async Task EnviarCorreoAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Mensaje = "Ingrese su correo electrónico.";
                return;
            }

            var enviado = await _authService.EnviarCorreoRecuperacionAsync(Email);

            if (enviado)
                Mensaje = "Revisa tu correo electrónico para restablecer tu contraseña.";
            else
                Mensaje = "No se pudo enviar el correo. Verifique el correo ingresado.";
        }

        [RelayCommand]
        private async Task CancelarAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
    }
