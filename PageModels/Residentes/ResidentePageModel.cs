using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiFirebase.PageModels.Residentes
{
    public partial class ResidentePageModel : ObservableObject
    {
        public ObservableCollection<Residente> ListaResidentes { get; } = new();

        [RelayCommand]
        public async Task CargarResidentesAsync()
        {
            ListaResidentes.Clear();
            ListaResidentes.Add(new Residente { NombreResidente = "Ejemplo", ApellidoResidente = "Prueba" });
        }

        public int IdResidenteSeleccionado { get; set; }

        // ✅ COMANDO PARA NAVEGAR AL FORMULARIO
        [RelayCommand]
        private async Task NavigateToRegister()
        {

            await Shell.Current.GoToAsync("residenteForm");
        }

        // ✅ COMANDO PARA NAVEGAR A LA LISTA
        [RelayCommand]
        private async Task NavigateToList()
        {

            await Shell.Current.GoToAsync("residenteList");
        }

    }
}
