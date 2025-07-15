using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using System.Text.Json;
using Timer = System.Timers.Timer;


namespace MauiFirebase.PageModels.Mapas;

public partial class UbicacionVehiculoPageModel : ObservableValidator, IDisposable
{
    private readonly IUbicacionVehiculo _ubicacionVehiculo;
    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly IRutaRepository _rutaRepository;
    private readonly FirebaseUbicacionService _firebaseUbicacionService;

    private readonly SincronizacionFirebaseService _sincronizador;
    private readonly RutaService _rutaService = new();
    public ObservableCollection<UbicacionVehiculo> Ubicaciones { get; } = new();
    public ObservableCollection<Vehiculo> ListaVehiculos { get; } = new();
    public ObservableCollection<Pin> MapaPins { get; } = new();
    private readonly Dictionary<string, Pin> _pinPorVehiculo = new();
    public ObservableCollection<Polyline> Rutas { get; } = new();
    public ObservableCollection<MapElement> RutasEnMapa { get; } = new();


    private Timer? _timer;
    private bool _isUpdating = false;
    private CancellationTokenSource _cts = new();

    [ObservableProperty]
    private bool isPolling;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private int pollingInterval = 5000;


    public UbicacionVehiculoPageModel(IUbicacionVehiculo ubicacionVehiculo,
        IVehiculoRepository vehiculoRepository,
        IRutaRepository rutaRepository,
        FirebaseUbicacionService firebaseUbicacionService,
        SincronizacionFirebaseService sincronizador)
    {
        _ubicacionVehiculo = ubicacionVehiculo;
        _vehiculoRepository = vehiculoRepository;
        _rutaRepository = rutaRepository;
        _sincronizador = sincronizador;
        _firebaseUbicacionService = firebaseUbicacionService; // 👈 asignación
        StartPolling();
    }

    public async Task InicializarDatosAsync()
    {
        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _sincronizador.SincronizarVehiculoDesdeFirebaseAsync();
                await _sincronizador.SincronizarUbicacionesDesdeFirebaseAsync();
                await _sincronizador.SincronizarRutasDesdeFirebaseAsync();
            }
            await CargarVehiculoAsync();
            await CargarUbicacionAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al inicializar datos: {ex.Message}";
        }
    }


    [RelayCommand]
    public async Task CargarVehiculoAsync()
    {
        ListaVehiculos.Clear();
        var vehiculos = await _vehiculoRepository.GetAllVehiculoAsync();
        foreach (var item in vehiculos)
        {
            ListaVehiculos.Add(item);
        }

    }

    [RelayCommand]
    public async Task CargarUbicacionAsync()
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            await _sincronizador.SincronizarVehiculoDesdeFirebaseAsync();
            await _sincronizador.SincronizarUbicacionesDesdeFirebaseAsync();
        }

        Ubicaciones.Clear();

        var ubicaciones = await _ubicacionVehiculo.ObtenerTodasAsync();
        var vehiculos = await _vehiculoRepository.GetAllVehiculoAsync();

        var vehiculosDict = vehiculos.ToDictionary(v => v.IdVehiculo);

        foreach (var ubicacion in ubicaciones)
        {
            if (vehiculosDict.TryGetValue(ubicacion.IdVehiculo!, out var vehiculo))
            {
                ubicacion.Placa = vehiculo.PlacaVehiculo;
                ubicacion.NombreConductor = vehiculo.Nombre;
            }
            Ubicaciones.Add(ubicacion);
        }
        await ActualizarUbicacionesAsync();
    }

    [RelayCommand]
    public async Task GuardarUbicacionAsync(UbicacionVehiculo ubicacion)
    {
        ubicacion.Sincronizado = false;
        await _ubicacionVehiculo.GuardarUbicacionAsync(ubicacion);

        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            await _sincronizador.SincronizarUbicacionesAsync();
        }

        await CargarUbicacionAsync();
    }

    public void AgregarPinSimuladoAndahuaylas()
    {
        double lat = -13.653820;
        double lng = -73.360519;

        var pin = new Pin
        {
            Label = "Placa: ABC-123",
            Address = "Marca: Volvo\nModelo: FH16\nConductor: Juan Pérez",
            Location = new Location(lat, lng),
            Type = PinType.Place
        };
        MapaPins.Add(pin);
    }

    public void InitializeTimer()
    {
        _timer = new Timer(PollingInterval);
        _timer.AutoReset = true;
        _timer.Elapsed += async (_, _) => await TimerElapsedAsync();
    }

    public async Task TimerElapsedAsync()
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


    public async Task<Location?> ObtenerUbicacionUsuarioAsync()
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.High);
            var location = await Geolocation.GetLocationAsync(request);

            if (location != null)
            {
                Console.WriteLine($"Usuario: {location.Latitude}, {location.Longitude}");
                return location;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error obteniendo ubicación del usuario: {ex.Message}");
        }

        return null;
    }

    public async Task ActualizarUbicacionesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            ErrorMessage = null;

            //var ubicaciones = await _ubicacionVehiculo.ObtenerTodasAsync();
            var vehiculos = await _vehiculoRepository.GetAllVehiculoAsync();
            // ubicacion del realtime database
            var ubicaciones = await ObtenerUbicacionesRealtimeAsync();
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            var vehiculosDict = vehiculos.ToDictionary(v => v.IdVehiculo!);
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Ubicaciones.Clear();
                MapaPins.Clear();
                _pinPorVehiculo.Clear();

                foreach (var u in ubicaciones)
                {
                    if (vehiculosDict.TryGetValue(u.IdVehiculo!, out var vehiculo))
                    {
                        u.Placa = vehiculo.PlacaVehiculo;
                        u.NombreConductor = vehiculo.Nombre;
                    }

                    Ubicaciones.Add(u);

                    var pin = new Pin
                    {
                        Label = $"🚛 {u.Placa ?? "Sin placa"} - {u.NombreConductor ?? "Sin conductor"}",
                        Address =
                                  $"📍 Lat: {u.Latitud:F6}, Lng: {u.Longitud:F6}",
                        Location = new Location(u.Latitud, u.Longitud),
                        Type = PinType.Place
                    };

                    _pinPorVehiculo[u.IdVehiculo ?? u.IdUbicacionVehiculo ?? Guid.NewGuid().ToString()] = pin;
                    MapaPins.Add(pin);
                }
                //var andahuaylasLat = -13.653820;
                //var andahuaylasLng = -73.360519;

                //var pinAndahuaylas = new Pin
                //{
                //    Label = "Placa: XYZ-999",
                //    Address = "Conductor: Juan Perez",
                //    Location = new Location(andahuaylasLat, andahuaylasLng),
                //    Type = PinType.Place
                //};

                //_pinPorVehiculo["AndahuaylasFijo"] = pinAndahuaylas;
                //MapaPins.Add(pinAndahuaylas);

                VerificarProximidad();
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error al cargar ubicaciones: {ex.Message}";
        }
    }
    // 
    // ubicacion desde realtime database
    public async Task<List<UbicacionVehiculo>> ObtenerUbicacionesRealtimeAsync()
    {
        try
        {
            var ubicaciones = await _firebaseUbicacionService.ObtenerTodasUbicacionesAsync();
            return ubicaciones;
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al consultar Realtime DB: " + ex.Message;
            return new List<UbicacionVehiculo>();
        }
    }

    private readonly TimeSpan intervaloNotificacion = TimeSpan.FromSeconds(20);
    private DateTime ultimaNotificacion = DateTime.MinValue;
    public async void VerificarProximidad()
    {
        var ubicacionUsuario = await ObtenerUbicacionUsuarioAsync();
        if (ubicacionUsuario == null)
        {
            return;
        }

        var ubicacionCamion = new Location(-13.653820, -73.360519);

        var distancia = Location.CalculateDistance(ubicacionUsuario, ubicacionCamion, DistanceUnits.Kilometers);

        if (distancia <= 0.5)
        {
            if ((DateTime.Now - ultimaNotificacion) > intervaloNotificacion)
            {
                ultimaNotificacion = DateTime.Now;

                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(20000));

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplaySnackbar(
                        message: "🚛 El camión está cerca!",
                        duration: TimeSpan.FromSeconds(15),
                        visualOptions: new SnackbarOptions
                        {
                            BackgroundColor = Colors.Gold,
                            TextColor = Colors.Black,
                            CornerRadius = 8,
                            Font = Microsoft.Maui.Font.SystemFontOfSize(17)
                        }
                    );
                });
            }
        }
        else
        {
            ultimaNotificacion = DateTime.MinValue;
        }
    }

    public async Task CargarRutaDelDiaAsync(string dia)
    {
        try
        {
            RutasEnMapa.Clear();

            var rutas = await _rutaRepository.GetAllRutaAsync();
            var rutaDelDia = rutas.FirstOrDefault(r =>
                r.DiasDeRecoleccion?.ToLower().Contains(dia.ToLower()) == true);

            if (rutaDelDia == null || string.IsNullOrWhiteSpace(rutaDelDia.PuntosRutaJson))
                return;

            var puntos = JsonSerializer.Deserialize<List<LatLng>>(rutaDelDia.PuntosRutaJson);
            if (puntos == null || puntos.Count < 2)
                return;
            var locations = puntos.Select(p => new Location(p.lat, p.lng)).ToList();
            var rutaGoogle = await _rutaService.ObtenerRutaGoogleAsync(locations);

            if (rutaGoogle == null || rutaGoogle.Count == 0)
                return;

            var polyline = new Polyline
            {
                StrokeColor = Colors.Blue,
                StrokeWidth = 6
            };

            foreach (var punto in rutaGoogle)
            {
                polyline.Geopath.Add(punto);
            }

            RutasEnMapa.Add(polyline);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al cargar ruta: " + ex.Message;
        }
    }


    public async Task RenderizarRutaEnMapaAsync(string idRuta)
    {
        try
        {
            RutasEnMapa.Clear();
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _sincronizador.SincronizarRutasDesdeFirebaseAsync();
            }
            var ruta = await _rutaRepository.GetRutaIdAsync(idRuta);
            if (ruta == null || string.IsNullOrWhiteSpace(ruta.PuntosRutaJson))
                return;
            var puntos = JsonSerializer.Deserialize<List<LatLng>>(ruta.PuntosRutaJson);
            if (puntos == null || puntos.Count < 2)
                return;
            var polyline = new Polyline
            {
                StrokeColor = Colors.Red,
                StrokeWidth = 5
            };
            foreach (var punto in puntos)
            {
                polyline.Geopath.Add(new Location(punto.lat, punto.lng));
            }
            RutasEnMapa.Add(polyline);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al renderizar ruta: " + ex.Message;
        }
    }

    private class LatLng
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    [RelayCommand]
    public void StartPolling()
    {
        if (_timer == null)
        {
            InitializeTimer();
        }

        if (!_timer!.Enabled)
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