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
    private readonly Polyline _polyline = new();

    public EditarRutaPage(EditarRutaPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _polyline.StrokeColor = Colors.Blue;
        _polyline.StrokeWidth = 4;
        MyMap.MapElements.Add(_polyline);

        MyMap.MapClicked += OnMapClicked;
        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(-13.651239, -73.363682), Distance.FromKilometers(5)));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InicializarAsync();

        if (!string.IsNullOrEmpty(_viewModel.PuntosRutaJson))
            await CargarPuntosDesdeJson();
    }

    private async Task CargarPuntosDesdeJson()
    {
        try
        {
            var puntos = JsonSerializer.Deserialize<List<JsonElement>>(_viewModel.PuntosRutaJson);
            if (puntos is null || puntos.Count == 0) return;

            _routePoints.Clear();

            foreach (var punto in puntos)
            {
                if (punto.TryGetProperty("lat", out var lat) &&
                    punto.TryGetProperty("lng", out var lng))
                {
                    _routePoints.Add(new Location(lat.GetDouble(), lng.GetDouble()));
                }
            }

            await ActualizarRutaEnMapa();

            if (_routePoints.Count > 0)
                MyMap.MoveToRegion(CalcularBounds(_routePoints));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar puntos: {ex.Message}", "OK");
        }
    }

    private MapSpan CalcularBounds(List<Location> puntos)
    {
        var latMin = puntos.Min(p => p.Latitude);
        var latMax = puntos.Max(p => p.Latitude);
        var lngMin = puntos.Min(p => p.Longitude);
        var lngMax = puntos.Max(p => p.Longitude);

        var center = new Location((latMin + latMax) / 2, (lngMin + lngMax) / 2);
        var radius = Math.Max(Math.Abs(latMax - latMin), Math.Abs(lngMax - lngMin)) * 111000 / 2;

        return MapSpan.FromCenterAndRadius(center, Distance.FromMeters(Math.Max(radius, 1000)));
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        _routePoints.Add(e.Location);
        await ActualizarRutaEnMapa();
    }

    private async Task ActualizarRutaEnMapa()
    {
        _polyline.Geopath.Clear();

        try
        {
            var ruta = _routePoints.Count >= 2
                ? await _rutaService.ObtenerRutaGoogleAsync(_routePoints)
                : _routePoints;

            foreach (var loc in ruta)
                _polyline.Geopath.Add(loc);
        }
        catch
        {
            foreach (var loc in _routePoints)
                _polyline.Geopath.Add(loc);
        }

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

    private async void OnUndoClicked(object sender, EventArgs e)
    {
        if (_routePoints.Count > 0)
        {
            _routePoints.RemoveAt(_routePoints.Count - 1);
            await ActualizarRutaEnMapa();
        }
    }

    private async void OnClearClicked(object sender, EventArgs e)
    {
        _routePoints.Clear();
        _polyline.Geopath.Clear();
        MyMap.Pins.Clear();
        _viewModel.PuntosRutaJson = string.Empty;
    }

    private void OnExportClicked(object sender, EventArgs e)
    {
        if (_routePoints.Count == 0)
        {
            DisplayAlert("Aviso", "No hay puntos para exportar.", "OK");
            return;
        }

        var coords = _routePoints.Select(p => new { lat = p.Latitude, lng = p.Longitude }).ToList();
        _viewModel.PuntosRutaJson = JsonSerializer.Serialize(coords, new JsonSerializerOptions { WriteIndented = true });

        DisplayAlert("Exportado", "Ruta exportada al JSON.", "OK");
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
