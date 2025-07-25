using MauiFirebase.PageModels.Mapas;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.Specialized;

namespace MauiFirebase.Pages.Mapa;

public partial class MonitorearCamionPage : ContentPage
{
    private readonly UbicacionVehiculoPageModel _viewModel;

    public MonitorearCamionPage(UbicacionVehiculoPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        _viewModel.MapaPins.CollectionChanged += MapaPins_CollectionChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.CargarUbicacionAsync();

        map.Pins.Clear();
        foreach (var pin in _viewModel.MapaPins)
        {
            map.Pins.Add(pin);
        }

        if (map.Pins.Count > 0)
        {
            var primerPin = map.Pins[0];
            map.MoveToRegion(MapSpan.FromCenterAndRadius(
                primerPin.Location,
                Distance.FromKilometers(0.5)
            ));
        }
        var diaActual = DateTime.Now.ToString("dddd", new System.Globalization.CultureInfo("es-PE"));
        map.MapElements.Clear();

        await _viewModel.CargarRutaDelDiaAsync(diaActual);
        foreach (var ruta in _viewModel.RutasEnMapa)
        {
            map.MapElements.Add(ruta);
        }
    }



    private void MapaPins_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (Pin pin in e.NewItems)
                {
                    if (!map.Pins.Contains(pin))
                    {
                        map.Pins.Add(pin);
                    }
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Location, Distance.FromKilometers(0.5)));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (Pin pin in e.OldItems)
                {
                    if (map.Pins.Contains(pin))
                        map.Pins.Remove(pin);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                map.Pins.Clear();
            }
        });
    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (_viewModel != null)
        {
            _viewModel.MapaPins.CollectionChanged -= MapaPins_CollectionChanged;
        }
    }
}