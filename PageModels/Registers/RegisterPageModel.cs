using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Registers
{
    public partial class RegisterPageModel : ObservableObject
    {
        [ObservableProperty]
        private string nombre;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [RelayCommand]
        private async Task RegistrarAsync()
        {
            // Lógica de registro aquí (agrega validaciones si deseas)
            await Application.Current.MainPage.DisplayAlert("Registro", "Usuario registrado correctamente", "OK");

            // ✅ Regresar al Login usando Navigation
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        [RelayCommand]
        private async Task CancelarAsync()
        {
            // ✅ Cancelar y regresar al Login
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
