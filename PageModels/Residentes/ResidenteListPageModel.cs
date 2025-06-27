using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Residentes
{
    public partial class ResidenteListPageModel : ObservableObject
    {
        private IClosePopup? _popupCloser; // 🔧 Instancia UI del popup actual
        private readonly IResidenteRepository _residenteRepository;
        private readonly IAlertaHelper _alertaHelper;

        private List<Residente> _todosLosResidentes = new(); // respaldo en memoria

        [ObservableProperty]
        private ObservableCollection<Residente> _listaResidentes = new();

        [ObservableProperty]
        private string _busquedaTexto = string.Empty;

        [ObservableProperty]
        private string _filtroEstadoResidente = "Todos";
        

        partial void OnFiltroEstadoResidenteChanged(string value)
        {
            AplicarFiltros();
        }
        partial void OnBusquedaTextoChanged(string value)
        {
            AplicarFiltros();
        }

        public ResidenteListPageModel(IResidenteRepository residenteRepository, IAlertaHelper alertaHelper)
        {
            _residenteRepository = residenteRepository;
            _alertaHelper = alertaHelper;


            // Carga diferida en OnAppearing o mediante comando explícito
            WeakReferenceMessenger.Default.Register<Residente, string>(
                this,
                "ResidenteActualizado",
                (recipient, residenteActualizado) =>
                {
                    var existente = ListaResidentes.FirstOrDefault(r => r.IdResidente == residenteActualizado.IdResidente);
                    if (existente != null)
                    {
                        existente.NombreResidente = residenteActualizado.NombreResidente;
                        existente.ApellidoResidente = residenteActualizado.ApellidoResidente;
                        existente.DniResidente = residenteActualizado.DniResidente;
                        existente.CorreoResidente = residenteActualizado.CorreoResidente;
                        existente.DireccionResidente = residenteActualizado.DireccionResidente;
                        existente.EstadoResidente = residenteActualizado.EstadoResidente;
                    }
                    else
                    {
                        ListaResidentes.Add(residenteActualizado);
                    }
                    // También actualiza el respaldo para que los filtros funcionen correctamente
                    var index = _todosLosResidentes.FindIndex(r => r.IdResidente == residenteActualizado.IdResidente);
                    if (index >= 0)
                        _todosLosResidentes[index] = residenteActualizado;
                    AplicarFiltros();
                });

        }

        // 🔹 Navegación
        [RelayCommand]
        private async Task NavigateToRegister()
        {
            await Shell.Current.GoToAsync("residenteForm");
        }

        [RelayCommand]
        private async Task NavigateToList()
        {
            await Shell.Current.GoToAsync("residenteList");
        }

        // 🔹 Cargar y mantener los residentes en memoria
        [RelayCommand]
        public async Task CargarResidentesAsync()
        {
            var residentes = await _residenteRepository.GetAllResidentesAsync();
            _todosLosResidentes = residentes.OrderBy(r => r.NombreResidente).ToList();
            AplicarFiltros();
        }

        // 🔹 Buscar por nombre o apellido
        [RelayCommand]
        public Task BuscarResidentesAsync()
        {
            AplicarFiltros(); // ahora trabaja sobre la lista en memoria
            return Task.CompletedTask;
        }

        // 🔹 Buscar por DNI (consulta individual)
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
                AplicarFiltros(); // mostrar la lista anterior
            }
        }
        // Establece el popup actual para poder cerrarlo desde el ViewModel
        public void SetPopupCloser(IClosePopup popup)
        {
            _popupCloser = popup;
        }
        [RelayCommand]
        public void AplicarFiltros()
        {
            IEnumerable<Residente> filtered = _todosLosResidentes;

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
            foreach (var res in filtered)
            {
                ListaResidentes.Add(res);
            }
        }

        // 🔹 Filtrado por texto y estado (en memoria)


        // 🔹 Cambiar estado activo/inactivo
        [RelayCommand]
        private async Task CambiarEstadoResidente(int idResidente)
        {
            var residente = await _residenteRepository.GetResidenteByIdAsync(idResidente);
            if (residente != null)
            {
                residente.EstadoResidente = !residente.EstadoResidente;
                await _residenteRepository.UpdateResidenteAsync(residente);

                await Shell.Current.DisplayAlert(
                    "Estado Actualizado",
                    $"El estado de {residente.NombreResidente} ahora es: {(residente.EstadoResidente ? "Activo" : "Inactivo")}",
                    "OK"
                );

                await CargarResidentesAsync(); // recargar lista actualizada
            }
        }

        // 🔹 Editar residente (navegación con parámetro)
        [RelayCommand]
        private async Task EditarResidente(Residente residente)
        {
            if (residente == null)
            {
                await Shell.Current.DisplayAlert("Error", "El residente recibido es null", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"residenteForm?id={residente.IdResidente}");

            _popupCloser?.ClosePopup(); // ✅ Cerramos el popup actual
            await _alertaHelper.ShowSuccessAsync("Ticket agregado correctamente.");
        }
    }
}
