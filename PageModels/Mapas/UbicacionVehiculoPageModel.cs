using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using Microsoft.Maui.Controls.Maps;
using System.Collections.ObjectModel;
using Timer = System.Timers.Timer;
namespace MauiFirebase.PageModels.Mapas;
public partial class UbicacionVehiculoPageModel : ObservableValidator, IDisposable
{
    private readonly IUbicacionVehiculo _ubicacionVehiculo;
    public ObservableCollection<UbicacionVehiculo> Ubicaciones { get; } = new();
    public ObservableCollection<Pin> MapaPins { get; } = new();
    private readonly Dictionary<int, Pin> _pinPorVehiculo = new();


    private Timer? _timer;
    private bool _isUpdating = false;
    private CancellationTokenSource _cts = new();

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private bool isPolling;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private int pollingInterval = 5000;

    private readonly Location destino = new Location(-12.0453, -77.0311);

    private bool yaNotificado = false;

    public UbicacionVehiculoPageModel(IUbicacionVehiculo ubicacionVehiculo)
    {
        _ubicacionVehiculo = ubicacionVehiculo;
        StartPolling();
    }

    private void InitializeTimer()
    {
        _timer = new Timer(PollingInterval);
        _timer.AutoReset = true;
        _timer.Elapsed += async (_, _) => await TimerElapsedAsync();
    }

    private async Task TimerElapsedAsync()
    {
        if (_isUpdating) return;

        try
        {
            _isUpdating = true;
            await ActualizarUbicacionesAsync(_cts.Token);
        }
        finally
        {
            _isUpdating = false;
        }
    }

    public async Task ActualizarUbicacionesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            IsRefreshing = true;
            ErrorMessage = null;

            var lista = await _ubicacionVehiculo.ObtenerTodasAsync();

            if (cancellationToken.IsCancellationRequested) return;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Ubicaciones.Clear();
                var idsActuales = new HashSet<int>(lista.Select(u => u.IdVehiculo));
                var pinsParaEliminar = _pinPorVehiculo.Keys
                    .Where(id => !idsActuales.Contains(id))
                    .ToList();

                foreach (var idEliminar in pinsParaEliminar)
                {
                    if (_pinPorVehiculo.TryGetValue(idEliminar, out var pin))
                    {
                        MapaPins.Remove(pin);
                        _pinPorVehiculo.Remove(idEliminar);
                    }
                }

                foreach (var u in lista)
                {
                    Ubicaciones.Add(u);

                    if (_pinPorVehiculo.TryGetValue(u.IdVehiculo, out var existingPin))
                    {
                        existingPin.Location = new Location(u.Latitud, u.Longitud);
                    }
                    else
                    {
                        var pin = new Pin
                        {
                            Label = $"Vehículo {u.IdVehiculo}",
                            Location = new Location(u.Latitud, u.Longitud),
                            Address = $"Lat: {u.Latitud}, Lng: {u.Longitud}"
                        };

                        _pinPorVehiculo[u.IdVehiculo] = pin;
                        MapaPins.Add(pin);
                    }
                }

                VerificarProximidad();
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar ubicaciones: {ex.Message}";
        }
        finally
        {
            IsRefreshing = false;
        }
    }


    private void VerificarProximidad()
    {
        foreach (var ubicacion in Ubicaciones)
        {
            var distancia = Location.CalculateDistance(
                destino,
                new Location(ubicacion.Latitud, ubicacion.Longitud),
                DistanceUnits.Kilometers);

            if (distancia <= 0.5) 
            {
                if (!yaNotificado)
                {
                    yaNotificado = true;
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.DisplayAlert("Aviso", "El camión está a menos de 500 metros del destino", "OK");
                    });
                }
            }
            else
            {
                yaNotificado = false;
            }
        }
    }

    [RelayCommand]
    public async Task RefrescarCommand()
    {
        await ActualizarUbicacionesAsync();
    }

    [RelayCommand]
    public void StartPolling()
    {
        if (_timer == null)
        {
            InitializeTimer();
        }

        if (!_timer.Enabled)
        {
            if (_cts.IsCancellationRequested)
            {
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }
            _timer.Interval = PollingInterval;
            _timer.Start();
            IsPolling = true;
        }
    }

    [RelayCommand]
    public void StopPolling()
    {
        if (_timer != null && _timer.Enabled)
        {
            _timer.Stop();
            IsPolling = false;
        }
    }

    [RelayCommand]
    public void CambiarIntervalo(int nuevoIntervalo)
    {
        PollingInterval = nuevoIntervalo;

        if (_timer != null)
        {
            _timer.Interval = nuevoIntervalo;
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _timer?.Stop();
        _timer?.Dispose();
        _cts.Dispose();
    }
}
