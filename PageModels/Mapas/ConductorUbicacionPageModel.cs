using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using MauiFirebase.Models;
using MauiFirebase.Services;
using MauiFirebase.Data.Interfaces;
using System.Timers;
using Microsoft.Maui.ApplicationModel;

namespace MauiFirebase.PageModels.Mapas
{
    public partial class ConductorUbicacionPageModel : ObservableValidator
    {
        private readonly FirebaseAuthService _authService;
        private readonly IVehiculoRepository _vehiculoRepository;
        private readonly FirebaseUbicacionService _ubicacionService;
        private System.Timers.Timer? _timer;

        private string? _uid;
        private Vehiculo? _vehiculo;
        private bool _isTracking = false;

        public ConductorUbicacionPageModel(
            FirebaseAuthService authService,
            IVehiculoRepository vehiculoRepository,
            FirebaseUbicacionService ubicacionService)
        {
            _authService = authService;
            _vehiculoRepository = vehiculoRepository;
            _ubicacionService = ubicacionService;
        }

        [ObservableProperty]
        private string estado = "Esperando...";
        [ObservableProperty]
        private bool isTrackingVisible;
        [ObservableProperty]
        public double? latitud;

        [ObservableProperty]
        public double? longitud;

        [RelayCommand]
        public async Task IniciarSeguimientoAsync()
        {
            if (_isTracking)
                return;

            Estado = "🔄 Iniciando seguimiento...";

            try
            {
                _uid = _authService.GetUserId();
                _vehiculo = await _vehiculoRepository.GetByUsuarioUidAsync(_uid);

                if (_vehiculo == null)
                {
                    Estado = "🚫 Vehículo no encontrado.";
                    return;
                }

                _timer = new System.Timers.Timer(5000);
                _timer.Elapsed += async (s, e) => await EnviarUbicacionAsync();
                _timer.AutoReset = true;
                _timer.Start();

                _isTracking = true;
                IsTrackingVisible = true; // 👈 Ahora sí se activa correctamente
                Estado = "✅ Enviando ubicación cada 5 segundos.";

            }
            catch (Exception ex)
            {
                Estado = $"❗ Error: {ex.Message}";
            }
        }

        [RelayCommand]
        public void DetenerSeguimiento()
        {
            if (!_isTracking)
                return;

            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;

            _isTracking = false;
            IsTrackingVisible = false;
            Estado = "🛑 Seguimiento detenido.";
        }


        private async Task EnviarUbicacionAsync()
        {
            try
            {
                // Ejecutar en el hilo principal
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var permisosOk = await VerificarPermisosUbicacionAsync();
                    if (!permisosOk) return;

                    var request = new GeolocationRequest(GeolocationAccuracy.High);
                    var location = await Geolocation.GetLocationAsync(request);
                    Latitud = location.Latitude;
                    Longitud = location.Longitude;

                    if (location == null) return;

                    var datosUbicacion = new
                    {
                        idVehiculo = _vehiculo!.IdVehiculo,
                        latitud = location.Latitude,
                        longitud = location.Longitude,
                        placa = _vehiculo.PlacaVehiculo,
                        nombreConductor = Preferences.Get("FirebaseUserNombre", "") + " " + Preferences.Get("FirebaseUserApellido", ""),
                        timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                    };

                    await _ubicacionService.EnviarUbicacionAsync(_uid!, datosUbicacion);
                });
            }
            catch (Exception ex)
            {
                Estado = $"⚠️ Error al enviar ubicación: {ex.Message}";
            }
        }


        private async Task<bool> VerificarPermisosUbicacionAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    // Mostrar alerta si el usuario negó el permiso
                    await Shell.Current.DisplayAlert(
                        "Permiso necesario",
                        "Debes permitir el acceso a la ubicación para usar esta funcionalidad.",
                        "Aceptar"
                    );
                    return false;
                }
            }

            return true;
        }


    }
}
