using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            // Aquí va la lógica de registro
            await Application.Current.MainPage.DisplayAlert("Registro", "Usuario registrado correctamente", "OK");

            // Luego de registrarse, podrías redirigir al login
            await Shell.Current.GoToAsync(".."); // Regresar a la página anterior
        }

        [RelayCommand]
        private async Task CancelarAsync()
        {
            await Shell.Current.GoToAsync(".."); // Regresar a LoginPage
        }
    }
}
