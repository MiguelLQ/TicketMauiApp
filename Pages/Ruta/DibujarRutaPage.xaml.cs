using MauiFirebase.PageModels.Rutas;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Text.Json;

namespace MauiFirebase.Pages.Ruta;

public partial class DibujarRutaPage : ContentPage
{
    private readonly CrearRutaPageModel _viewModel;
    private readonly RutaService _rutaService = new();
    private readonly List<Location> _routePoints = new();
    private readonly Polyline _polyline = new() { StrokeColor = Colors.Blue, StrokeWidth = 4 };

    public DibujarRutaPage(CrearRutaPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        MyMap.MapElements.Add(_polyline);
        MyMap.MapClicked += OnMapClicked!;
        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(-13.651239, -73.363682), Distance.FromKilometers(0.3)));
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        _routePoints.Add(e.Location);
        await ActualizarRutaEnMapa();
    }

    private async Task ActualizarRutaEnMapa()
    {
        _polyline.Geopath.Clear();

        if (_routePoints.Count < 2)
        {
            foreach (var pt in _routePoints)
                _polyline.Geopath.Add(pt);
        }
        else
        {
            var rutaReal = await _rutaService.ObtenerRutaGoogleAsync(_routePoints);
            foreach (var loc in rutaReal)
                _polyline.Geopath.Add(loc);
        }

        MyMap.Pins.Clear();
        for (int i = 0; i < _routePoints.Count; i++)
        {
            MyMap.Pins.Add(new Pin
            {
                Location = _routePoints[i],
                Label = $"Punto {i + 1}",
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

    private void OnClearClicked(object sender, EventArgs e)
    {
        _routePoints.Clear();
        _polyline.Geopath.Clear();
        MyMap.Pins.Clear();
        _viewModel.PuntosRutaJson = null;
    }

    private async void OnExportClicked(object sender, EventArgs e)
    {
        if (_routePoints.Count == 0)
        {
            await DisplayAlert("Aviso", "No hay puntos para exportar.", "OK");
            return;
        }

        var coords = _routePoints
            .Select(p => new { lat = p.Latitude, lng = p.Longitude })
            .ToList();

        string json = JsonSerializer.Serialize(coords, new JsonSerializerOptions { WriteIndented = true });
        _viewModel.PuntosRutaJson = json;

        await DisplayAlert("Exportado", "Ruta exportada al campo JSON.", "OK");

        await Shell.Current.GoToAsync("AgregarRutaPage");

    }

    private void MoverMapa(double latDelta, double lngDelta)
    {
        if (MyMap.VisibleRegion == null) return;

        var center = MyMap.VisibleRegion.Center;
        var nuevoCentro = new Location(center.Latitude + latDelta, center.Longitude + lngDelta);
        MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(nuevoCentro, MyMap.VisibleRegion.Radius));
    }
}
