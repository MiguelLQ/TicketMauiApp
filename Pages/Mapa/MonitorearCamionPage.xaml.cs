using MauiFirebase.PageModels.Mapas;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.Specialized;

namespace MauiFirebase.Pages.Mapa;

public partial class MonitorearCamionPage : ContentPage
{
    private readonly UbicacionVehiculoPageModel _viewModel;
    private bool _isInitialPositionSet = false;

    public MonitorearCamionPage(UbicacionVehiculoPageModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        _viewModel.MapaPins.CollectionChanged += MapaPins_CollectionChanged;
        _viewModel.Ubicaciones.CollectionChanged += (_, __) => MoveToFirstPin();
    }

    private void MapaPins_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            map.Pins.Clear();
            foreach (var pin in _viewModel.MapaPins)
            {
                map.Pins.Add(pin);
            }
        });
    }

    private void MoveToFirstPin()
    {
        if (_isInitialPositionSet)
        {
            return;
        }

        if (_viewModel.MapaPins.Count > 0)
        {
            var first = _viewModel.MapaPins[0];
            map.MoveToRegion(MapSpan.FromCenterAndRadius(first.Location, Distance.FromKilometers(5)));
            _isInitialPositionSet = true;
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.MapaPins.CollectionChanged -= MapaPins_CollectionChanged;
    }
}
