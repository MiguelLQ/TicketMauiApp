// =================== ResidenteListPageModel.cs ===================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Para Shell.Current.GoToAsync

namespace MauiFirebase.PageModels.Residentes
{
    public partial class ResidenteListPageModel : ObservableObject
    {
        private readonly IResidenteRepository _residenteRepository;

        [ObservableProperty]
        private ObservableCollection<Residente> _listaResidentes = new();

        [ObservableProperty]
        private string _busquedaTexto = string.Empty;

        [ObservableProperty]
        private string _filtroEstadoResidente = "Todos"; // "Todos", "Activos", "Inactivos"

        public ResidenteListPageModel(IResidenteRepository residenteRepository)
        {
            _residenteRepository = residenteRepository;
            // No cargamos aquí, se carga en OnAppearing de la página o con un comando
        }

        // Comandos de navegación para la página principal
        [RelayCommand]
        private async Task NavigateToRegister()
        {
            // Navega a la página del formulario (vacío para nuevo registro)
            await Shell.Current.GoToAsync("residenteForm");
        }

        [RelayCommand]
        private async Task NavigateToList()
        {
            // Navega a la página de la lista
            await Shell.Current.GoToAsync("residenteList");
        }

        // Comandos para la lista
        [RelayCommand]
        public async Task CargarResidentesAsync()
        {
            var residentes = await _residenteRepository.GetAllResidentesAsync();
            ListaResidentes.Clear();
            foreach (var residente in residentes.OrderBy(r => r.NombreResidente)) // Ordenar por nombre
            {
                ListaResidentes.Add(residente);
            }
            AplicarFiltros(); // Asegura que los filtros iniciales se apliquen
        }

        [RelayCommand]
        public async Task BuscarResidentesAsync()
        {
            await CargarResidentesAsync(); // Recargar todos para buscar sobre la lista completa
            if (!string.IsNullOrWhiteSpace(BusquedaTexto))
            {
                var filteredList = ListaResidentes
                    .Where(r => r.NombreResidente.Contains(BusquedaTexto, StringComparison.OrdinalIgnoreCase) ||
                                r.ApellidoResidente.Contains(BusquedaTexto, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                ListaResidentes.Clear();
                foreach (var res in filteredList)
                {
                    ListaResidentes.Add(res);
                }
            }
            else
            {
                await CargarResidentesAsync(); // Si la búsqueda está vacía, recarga todos y aplica filtros
            }
            AplicarFiltros(); // Aplicar filtros después de la búsqueda
        }

        [RelayCommand]
        public async Task BuscarResidentePorDniAsync()
        {
            if (string.IsNullOrWhiteSpace(BusquedaTexto))
            {
                await Shell.Current.DisplayAlert("Advertencia", "Por favor, ingrese un DNI para buscar.", "OK");
                return;
            }

            var residente = await _residenteRepository.GetResidenteByDniAsync(BusquedaTexto);
            ListaResidentes.Clear();
            if (residente != null)
            {
                ListaResidentes.Add(residente);
            }
            else
            {
                await Shell.Current.DisplayAlert("Información", "No se encontró ningún residente con ese DNI.", "OK");
                await CargarResidentesAsync(); // Recargar la lista completa si no se encuentra
            }
            AplicarFiltros(); // Aplicar filtros después de la búsqueda por DNI
        }

        [RelayCommand]
        public async Task AplicarFiltros()
        {
            var residentes = await _residenteRepository.GetAllResidentesAsync();
            var filtered = residentes.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(BusquedaTexto))
            {
                filtered = filtered.Where(r =>
                    r.NombreResidente.Contains(BusquedaTexto, StringComparison.OrdinalIgnoreCase) ||
                    r.ApellidoResidente.Contains(BusquedaTexto, StringComparison.OrdinalIgnoreCase));
            }

            if (FiltroEstadoResidente == "Activos")
                filtered = filtered.Where(r => r.EstadoResidente);
            else if (FiltroEstadoResidente == "Inactivos")
                filtered = filtered.Where(r => !r.EstadoResidente);

            ListaResidentes.Clear();
            foreach (var res in filtered.OrderBy(r => r.NombreResidente))
            {
                ListaResidentes.Add(res);
            }
        }


        [RelayCommand]
        private async Task CambiarEstadoResidente(int idResidente)
        {
            var residente = await _residenteRepository.GetResidenteByIdAsync(idResidente);
            if (residente != null)
            {
                residente.EstadoResidente = !residente.EstadoResidente; // Cambiar el estado
                await _residenteRepository.UpdateResidenteAsync(residente);
                await Shell.Current.DisplayAlert("Estado Actualizado", $"El estado de {residente.NombreResidente} ahora es: {(residente.EstadoResidente ? "Activo" : "Inactivo")}", "OK");
                await CargarResidentesAsync(); // Recargar la lista para reflejar el cambio
            }
        }

        [RelayCommand]
        private async Task EditarResidente(Residente residente)

        {
            if (residente == null)
            {
                await Shell.Current.DisplayAlert("Error", "El residente recibido es null", "OK");
                return;
            }

          
            await Shell.Current.DisplayAlert("Editar", $"Residente seleccionado: {residente.NombreResidente}", "OK");
            await Shell.Current.GoToAsync($"residenteForm?id={residente.IdResidente}");
        }


    }
}