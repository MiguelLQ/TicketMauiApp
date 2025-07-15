using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using MauiFirebase.Models;
using MauiFirebase.Services;
using MauiFirebase.Data.Interfaces;
using System.Timers;

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

            Estado = "🛑 Seguimiento detenido.";
        }

        private async Task EnviarUbicacionAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location == null) return;

                var datosUbicacion = new
                {
                    idVehiculo = _vehiculo!.IdVehiculo,
                    latitud = location.Latitude,
                    longitud = location.Longitude,
                    placa = _vehiculo.PlacaVehiculo,
                    nombreConductor = _vehiculo.Nombre,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                await _ubicacionService.EnviarUbicacionAsync(_uid!, datosUbicacion);
            }
            catch (Exception ex)
            {
                Estado = $"⚠️ Error al enviar ubicación: {ex.Message}";
            }
        }
    }
}
