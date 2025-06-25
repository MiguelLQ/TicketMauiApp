// =================== ResidenteFormPageModel.cs ===================

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls; // Necesario para Shell.Current.GoToAsync y QueryProperty

namespace MauiFirebase.PageModels.Residentes
{
    [QueryProperty(nameof(IdResidente), "id")]

    public partial class ResidenteFormPageModel : ObservableObject
    {
        private readonly IResidenteRepository _residenteRepository;

        [ObservableProperty]
        private int _idResidente;

        partial void OnIdResidenteChanged(int value)
        {
            if (value > 0)
                _ = CargarResidenteParaEdicion(value);
        }

        [ObservableProperty]
        private string _nombreResidente = string.Empty;

        [ObservableProperty]
        private string _apellidoResidente = string.Empty;

        [ObservableProperty]
        private string _dniResidente = string.Empty;

        [ObservableProperty]
        private string _correoResidente = string.Empty;

        [ObservableProperty]
        private string _direccionResidente = string.Empty;

        [ObservableProperty]
        private bool _estadoResidente = true; // Por defecto activo para nuevos

        // Constructor para Inyección de Dependencias
        public ResidenteFormPageModel(IResidenteRepository residenteRepository)
        {
            _residenteRepository = residenteRepository;
            // Opcional: Puedes llamar a LimpiarFormulario() aquí si quieres asegurar
            // que siempre inicie limpio, incluso si no se usa QueryProperty al principio.
            // LimpiarFormulario();
        }

        // Comandos para el formulario
        [RelayCommand]
        private async Task CrearResidenteAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreResidente) || string.IsNullOrWhiteSpace(ApellidoResidente) || string.IsNullOrWhiteSpace(DniResidente))
            {
                await Shell.Current.DisplayAlert("Error", "Nombre, Apellido y DNI son campos obligatorios.", "OK");
                return;
            }

            var newResidente = new Residente
            {
                NombreResidente = NombreResidente,
                ApellidoResidente = ApellidoResidente,
                DniResidente = DniResidente,
                CorreoResidente = CorreoResidente,
                DireccionResidente = DireccionResidente,
                EstadoResidente = EstadoResidente,
                FechaRegistroResidente = DateTime.Now
            };

            await _residenteRepository.CreateResidenteAsync(newResidente);
            await Shell.Current.DisplayAlert("Éxito", "Residente creado correctamente.", "OK");
            // LimpiarFormulario(); // Comentar aquí, ya que se limpiará al navegar de vuelta
            await Shell.Current.GoToAsync(".."); // Regresar a la lista o pantalla anterior
        }

        [RelayCommand]
        private async Task ActualizarResidenteAsync()
        {
            if (IdResidente == 0) // No hay residente seleccionado para actualizar
            {
                await Shell.Current.DisplayAlert("Error", "No hay residente seleccionado para actualizar.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(NombreResidente) || string.IsNullOrWhiteSpace(ApellidoResidente) || string.IsNullOrWhiteSpace(DniResidente))
            {
                await Shell.Current.DisplayAlert("Error", "Nombre, Apellido y DNI son campos obligatorios.", "OK");
                return;
            }

            // Primero, obtenemos el residente existente para mantener su FechaRegistroResidente original
            var existingResidente = await _residenteRepository.GetResidenteByIdAsync(IdResidente);
            if (existingResidente == null)
            {
                await Shell.Current.DisplayAlert("Error", "Residente no encontrado para actualizar.", "OK");
                return;
            }

            // Creamos un nuevo objeto Residente con los datos actualizados, manteniendo la fecha de registro original.
            var residenteToUpdate = new Residente
            {
                IdResidente = IdResidente,
                NombreResidente = NombreResidente,
                ApellidoResidente = ApellidoResidente,
                DniResidente = DniResidente,
                CorreoResidente = CorreoResidente,
                DireccionResidente = DireccionResidente,
                EstadoResidente = EstadoResidente,
                FechaRegistroResidente = existingResidente.FechaRegistroResidente // Mantenemos la fecha original
            };

            await _residenteRepository.UpdateResidenteAsync(residenteToUpdate);
            await Shell.Current.DisplayAlert("Éxito", "Residente actualizado correctamente.", "OK");
            // LimpiarFormulario(); // Comentar aquí, ya que se limpiará al navegar de vuelta
            await Shell.Current.GoToAsync(".."); // Regresar a la lista
        }

        [RelayCommand]
        private void LimpiarFormulario()
        {
            IdResidente = 0; // Importante para indicar que es un nuevo registro
            NombreResidente = string.Empty;
            ApellidoResidente = string.Empty;
            DniResidente = string.Empty;
            CorreoResidente = string.Empty;
            DireccionResidente = string.Empty;
            EstadoResidente = true;
        }

        // Método para cargar un residente en el formulario para edición
        // Este método será llamado internamente por la propiedad ResidenteId al establecerse.
        public async Task CargarResidenteParaEdicion(int id)
        {
            var residente = await _residenteRepository.GetResidenteByIdAsync(id);
            if (residente != null)
            {
                IdResidente = residente.IdResidente; // Asigna el Id localmente para actualizar
                NombreResidente = residente.NombreResidente;
                ApellidoResidente = residente.ApellidoResidente;
                DniResidente = residente.DniResidente;
                CorreoResidente = residente.CorreoResidente;
                DireccionResidente = residente.DireccionResidente;
                EstadoResidente = residente.EstadoResidente;
                // No asignes FechaRegistroResidente aquí, ya que es la fecha original y no debe editarse.
                // Si la necesitaras para mostrar, podrías añadir otra ObservableProperty.
            }
        }
    }
}