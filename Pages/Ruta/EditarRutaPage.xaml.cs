using MauiFirebase.PageModels.Rutas;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Text.Json;

namespace MauiFirebase.Pages.Ruta;

public partial class EditarRutaPage : ContentPage
{
    private readonly EditarRutaPageModel _viewModel;
    private readonly RutaService _rutaService = new RutaService();

    private readonly List<Location> _routePoints = new();
    private Polyline _polyline;
    private double desplazamiento = 0.001;
    private bool _mapaInicializado = false;

    public EditarRutaPage(EditarRutaPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Inicializar el mapa y la polilínea
        _polyline = new Polyline
        {
            StrokeColor = Colors.Blue,
            StrokeWidth = 4
        };
        MyMap.MapElements.Add(_polyline);

        // Configurar eventos del mapa
        MyMap.MapClicked += OnMapClicked;

        // Centrar el mapa en Andahuaylas
        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(-13.651239, -73.363682), Distance.FromKilometers(5)));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InicializarAsync();

        // Cargar puntos existentes en el mapa después de inicializar el ViewModel
        if (!_mapaInicializado && !string.IsNullOrEmpty(_viewModel.PuntosRutaJson))
        {
            await CargarPuntosDesdeJson();
            _mapaInicializado = true;
        }
    }

    // Cargar puntos desde el JSON del ViewModel al mapa
    private async Task CargarPuntosDesdeJson()
    {
        try
        {
            if (string.IsNullOrEmpty(_viewModel.PuntosRutaJson))
                return;

            var puntos = JsonSerializer.Deserialize<List<dynamic>>(_viewModel.PuntosRutaJson);
            if (puntos == null || puntos.Count == 0)
                return;

            _routePoints.Clear();

            foreach (var punto in puntos)
            {
                var jsonElement = (JsonElement)punto;
                if (jsonElement.TryGetProperty("lat", out var latElement) &&
                    jsonElement.TryGetProperty("lng", out var lngElement))
                {
                    var lat = latElement.GetDouble();
                    var lng = lngElement.GetDouble();
                    _routePoints.Add(new Location(lat, lng));
                }
            }

            await ActualizarRutaEnMapa();

            // Centrar el mapa en los puntos cargados
            if (_routePoints.Count > 0)
            {
                var bounds = CalcularBounds(_routePoints);
                MyMap.MoveToRegion(bounds);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar puntos desde JSON: {ex.Message}", "OK");
        }
    }

    // Calcular bounds para centrar el mapa en todos los puntos
    private MapSpan CalcularBounds(List<Location> puntos)
    {
        if (puntos.Count == 0)
            return MapSpan.FromCenterAndRadius(new Location(-13.651239, -73.363682), Distance.FromKilometers(5));

        var latMin = puntos.Min(p => p.Latitude);
        var latMax = puntos.Max(p => p.Latitude);
        var lngMin = puntos.Min(p => p.Longitude);
        var lngMax = puntos.Max(p => p.Longitude);

        var center = new Location((latMin + latMax) / 2, (lngMin + lngMax) / 2);
        var latDelta = Math.Abs(latMax - latMin);
        var lngDelta = Math.Abs(lngMax - lngMin);
        var radius = Math.Max(latDelta, lngDelta) * 111000 / 2; // Aproximado en metros

        return MapSpan.FromCenterAndRadius(center, Distance.FromMeters(Math.Max(radius, 1000)));
    }

    // Cuando el usuario toca el mapa, agrega punto
    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        _routePoints.Add(e.Location);
        await ActualizarRutaEnMapa();
    }

    // Actualiza la polilínea con la ruta calculada
    private async Task ActualizarRutaEnMapa()
    {
        if (_routePoints.Count < 2)
        {
            // Menos de 2 puntos: dibuja línea recta simple
            _polyline.Geopath.Clear();
            foreach (var pt in _routePoints)
                _polyline.Geopath.Add(pt);
        }
        else
        {
            // 2 o más puntos: obtiene ruta real y dibuja
            try
            {
                var rutaReal = await _rutaService.ObtenerRutaGoogleAsync(_routePoints);
                _polyline.Geopath.Clear();
                foreach (var loc in rutaReal)
                    _polyline.Geopath.Add(loc);
            }
            catch (Exception ex)
            {
                // Si falla la API, dibuja línea recta
                _polyline.Geopath.Clear();
                foreach (var pt in _routePoints)
                    _polyline.Geopath.Add(pt);
            }
        }

        // Actualiza los pines en el mapa
        MyMap.Pins.Clear();
        int i = 1;
        foreach (var pt in _routePoints)
        {
            MyMap.Pins.Add(new Pin
            {
                Location = pt,
                Label = $"Punto {i++}",
                Type = PinType.Place
            });
        }
    }

    // Botón para deshacer el último punto
    private async void OnUndoClicked(object sender, EventArgs e)
    {
        if (_routePoints.Count > 0)
        {
            _routePoints.RemoveAt(_routePoints.Count - 1);
            await ActualizarRutaEnMapa();
        }
    }

    // Botón para limpiar todos los puntos
    private async void OnClearClicked(object sender, EventArgs e)
    {
        _routePoints.Clear();
        _polyline.Geopath.Clear();
        MyMap.Pins.Clear();
        _viewModel.PuntosRutaJson = string.Empty;
    }

    // Botón para exportar puntos a JSON
    private void OnExportClicked(object sender, EventArgs e)
    {
        if (_routePoints.Count == 0)
        {
            DisplayAlert("Aviso", "No hay puntos para exportar.", "OK");
            return;
        }

        var coords = _routePoints.Select(p => new { lat = p.Latitude, lng = p.Longitude }).ToList();
        string json = JsonSerializer.Serialize(coords, new JsonSerializerOptions { WriteIndented = true });

        _viewModel.PuntosRutaJson = json;
        DisplayAlert("Exportado", "Ruta exportada al campo JSON.", "OK");
    }

    // Botón para cargar puntos desde el JSON del Editor
    private async void OnCargarJsonClicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrEmpty(_viewModel.PuntosRutaJson))
            {
                await DisplayAlert("Aviso", "No hay JSON para cargar.", "OK");
                return;
            }

            var puntos = JsonSerializer.Deserialize<List<dynamic>>(_viewModel.PuntosRutaJson);
            if (puntos == null || puntos.Count == 0)
            {
                await DisplayAlert("Aviso", "El JSON no contiene puntos válidos.", "OK");
                return;
            }

            _routePoints.Clear();

            foreach (var punto in puntos)
            {
                var jsonElement = (JsonElement)punto;
                if (jsonElement.TryGetProperty("lat", out var latElement) &&
                    jsonElement.TryGetProperty("lng", out var lngElement))
                {
                    var lat = latElement.GetDouble();
                    var lng = lngElement.GetDouble();
                    _routePoints.Add(new Location(lat, lng));
                }
            }

            await ActualizarRutaEnMapa();

            // Centrar el mapa en los puntos cargados
            if (_routePoints.Count > 0)
            {
                var bounds = CalcularBounds(_routePoints);
                MyMap.MoveToRegion(bounds);
            }

            await DisplayAlert("Éxito", "Puntos cargados desde JSON al mapa.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar JSON: {ex.Message}", "OK");
        }
    }

    // Métodos para mover el mapa
    private void MoverMapa(double latitudDelta, double longitudDelta)
    {
        if (MyMap.VisibleRegion == null) return;

        var center = MyMap.VisibleRegion.Center;
        var nuevoCentro = new Location(center.Latitude + latitudDelta, center.Longitude + longitudDelta);

        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(nuevoCentro, MyMap.VisibleRegion.Radius));
    }

    private void OnMoveUpClicked(object sender, EventArgs e)
    {
        MoverMapa(desplazamiento, 0);
    }

    private void OnMoveDownClicked(object sender, EventArgs e)
    {
        MoverMapa(-desplazamiento, 0);
    }

    private void OnMoveLeftClicked(object sender, EventArgs e)
    {
        MoverMapa(0, -desplazamiento);
    }

    private void OnMoveRightClicked(object sender, EventArgs e)
    {
        MoverMapa(0, desplazamiento);
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}