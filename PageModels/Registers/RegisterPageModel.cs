using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;


namespace MauiFirebase.PageModels.Registers;

public partial class RegisterPageModel : ObservableObject
{
    private readonly FirebaseAuthService _authService = new FirebaseAuthService();

    [ObservableProperty]
    private string? email;

    [ObservableProperty]
    private string? password;

    [ObservableProperty]
    private string? confirmarPassword;

    [RelayCommand]
    private async Task RegistrarAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmarPassword))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Por favor, complete todos los campos.", "OK");
            return;
        }

        if (Password != ConfirmarPassword)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
            return;
        }

        var uid = await _authService.RegistrarAuthUsuarioAsync(Email, Password);

        if (uid == null)
        {
            await Application.Current!.MainPage.DisplayAlert("Error", "No se pudo registrar. Verifica el correo o intenta más tarde.", "OK");
            return;
        }
        //
        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Ingrese un correo válido.", "OK");
            return;
        }


        Preferences.Set("FirebaseUserId", uid);
        Preferences.Set("FirebaseUserEmail", Email);

        await Application.Current.MainPage.DisplayAlert("Éxito", "Cuenta creada correctamente. Se ha enviado un correo de verificación. Revise su bandeja de entrada antes de iniciar sesión.Si no lo ves en tu bandeja de entrada, revisa tu carpeta de spam o promociones", "OK");


        // ✅ Volver al login (página anterior)
        await Application.Current.MainPage.Navigation.PopAsync();
    }

    [RelayCommand]
    private async Task CancelarAsync()
    {
        await Application.Current.MainPage.Navigation.PopAsync();
    }
}
