using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Helpers.Interface;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Vehiculos;

public partial class VehiculoPageModel : ObservableObject
{
    public ObservableCollection<Vehiculo> ListaVehiculos { get; } = new();

    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAlertaHelper _alertaHelper;
    private readonly SincronizacionFirebaseService _sincronizacionFirebaseService;

    [ObservableProperty]
    private bool isBusy;

    public VehiculoPageModel(
        IVehiculoRepository vehiculoRepository,
        IUsuarioRepository usuarioRepository,
        IAlertaHelper alertaHelper,
        SincronizacionFirebaseService sincronizacionFirebaseService)
    {
        _vehiculoRepository = vehiculoRepository;
        _usuarioRepository = usuarioRepository;
        _alertaHelper = alertaHelper;
        _sincronizacionFirebaseService = sincronizacionFirebaseService;
    }

    [RelayCommand]
    public async Task CargarVehiculosAsync()
    {
        try
        {
            IsBusy = true;
            await _sincronizacionFirebaseService.SincronizarVehiculosAsync();
            ListaVehiculos.Clear();

            var vehiculos = await _vehiculoRepository.GetAllVehiculoAsync();
            var usuarios = await _usuarioRepository.GetUsuariosAsync();

            foreach (var vehiculo in vehiculos)
            {
                Usuario? usuario = usuarios.FirstOrDefault(u => u.Uid == vehiculo.IdUsuario);
                vehiculo.Nombre = usuario?.Nombre ?? "Usuario no encontrado";

                ListaVehiculos.Add(vehiculo);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CambiarEstadoVehiculoAsync(string id)
    {
        await _vehiculoRepository.ChangeEstadoVehiculoAsync(id);
        await _alertaHelper.ShowSuccessAsync("Se cambió el estado del vehículo correctamente.");
        await CargarVehiculosAsync();
    }

    [RelayCommand]
    public async Task IrACrearVehiculoAsync()
    {
        await Shell.Current.GoToAsync("AgregarVehiculoPage");
    }

    [RelayCommand]
    public async Task IrAEditarVehiculoAsync(int idVehiculo)
    {
        await Shell.Current.GoToAsync($"EditarVehiculoPage?id={idVehiculo}");
    }
}
