using System.ComponentModel;
using MauiFirebase.PageModels.Mapas;

namespace MauiFirebase.Pages.Mapa;

public partial class EnviarUbicacionPage : ContentPage
{
    private readonly ConductorUbicacionPageModel _viewModel;
    private bool _isAnimating = false;

    public EnviarUbicacionPage(ConductorUbicacionPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.IsTrackingVisible))
        {
            if (_viewModel.IsTrackingVisible)
                IniciarAnimacionPulso();
            else
                _isAnimating = false;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Si ya está en modo seguimiento, arrancar animación directamente
        if (_viewModel.IsTrackingVisible)
            IniciarAnimacionPulso();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isAnimating = false;
    }

    private async void IniciarAnimacionPulso()
    {
        _isAnimating = true;

        // Iniciar 3 animaciones en paralelo
        _ = AnimarOnda(Pulse1, 0);
        _ = AnimarOnda(Pulse2, 400); // delay en milisegundos
        _ = AnimarOnda(Pulse3, 800);
    }

    private async Task AnimarOnda(Border circle, int delay)
    {
        await Task.Delay(delay);

        while (_isAnimating)
        {
            circle.Scale = 0.5;
            circle.Opacity = 1;

            var fadeTask = circle.FadeTo(0, 2000);
            var scaleTask = circle.ScaleTo(2.0, 2000, Easing.SinInOut);

            await Task.WhenAll(fadeTask, scaleTask);
            await Task.Delay(200); // breve pausa antes de reiniciar
        }
    }


}
