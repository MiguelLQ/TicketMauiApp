using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Rutas;

public partial class RutaPageModel : ObservableObject
{
    public ObservableCollection<Ruta> ListaRutas { get; } = new();

    private readonly IRutaRepository _rutaRepository;
    private readonly IAlertaHelper _alertaHelper;
    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly SincronizacionFirebaseService _sincronizador;

    [ObservableProperty]
    private bool isBusy;

    public RutaPageModel(IRutaRepository rutaRepository,
        IAlertaHelper alertaHelper,
        IVehiculoRepository vehiculoRepository,
        SincronizacionFirebaseService sincronizador)
    {
        _rutaRepository = rutaRepository;
        _alertaHelper = alertaHelper;
        _sincronizador = sincronizador;
        _vehiculoRepository = vehiculoRepository;
    }

    [RelayCommand]
    public async Task CargarRutasAsync()
    {
        try
        {
            IsBusy = true;

            if (_sincronizador != null && Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _sincronizador.SincronizarRutasDesdeFirebaseAsync();
            }

            ListaRutas.Clear();
            var rutas = await _rutaRepository.GetAllRutaAsync();

            foreach (var ruta in rutas)
            {
                if (!string.IsNullOrEmpty(ruta.IdVehiculo))
                {
                    var vehiculo = await _vehiculoRepository.GetVehiculoByIdAsync(ruta.IdVehiculo);
                    ruta.PlacaVehiculo = vehiculo?.PlacaVehiculo;
                }

                ListaRutas.Add(ruta);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CambiarEstadoRutaAsync(string id)
    {
        await _rutaRepository.ChangeEstadoRutaAsync(id);
        await _alertaHelper.ShowSuccessAsync("Se cambió el estado de manera exitosa");
        await CargarRutasAsync();
    }

    [RelayCommand]
    public async Task IrACrearRutaAsync()
    {
        await Shell.Current.GoToAsync("AgregarRutaPage");
    }

    [RelayCommand]
    public async Task GuardarRutaAsync(Ruta ruta)
    {
        ruta.Sincronizado = false;
        await _rutaRepository.CreateRutaAsync(ruta);
        if (_sincronizador != null && Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            await _sincronizador.SincronizarRutasAsync();
        }
        await CargarRutasAsync();
    }
}
