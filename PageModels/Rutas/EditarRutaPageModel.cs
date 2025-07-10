using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;

namespace MauiFirebase.PageModels.Rutas;

[QueryProperty(nameof(IdRuta), "id")]
public partial class EditarRutaPageModel : ObservableValidator
{
    private readonly IRutaRepository _rutaRepository;
    private readonly IAlertaHelper _alertaHelper;

    [ObservableProperty]
    private int idRuta;

    [ObservableProperty]
    
    private Ruta? _rutaSeleccionada;

    [ObservableProperty]
    private int _idVehiculo;

    [ObservableProperty]
    private string? _diasDeRecoleccion;

    [ObservableProperty]
    private bool _estadoRuta;

    [ObservableProperty]
    private string? _puntosRutaJson;

    public EditarRutaPageModel(IRutaRepository rutaRepository, IAlertaHelper alertaHelper)
    {
        _rutaRepository = rutaRepository;
        _alertaHelper = alertaHelper;
    }

    public async Task InicializarAsync()
    {
        RutaSeleccionada = await _rutaRepository.GetRutaIdAsync(IdRuta);
        if (RutaSeleccionada != null)
        {
            IdVehiculo = RutaSeleccionada.IdVehiculo;
            DiasDeRecoleccion = RutaSeleccionada.DiasDeRecoleccion;
            EstadoRuta = RutaSeleccionada.EstadoRuta;
            PuntosRutaJson = RutaSeleccionada.PuntosRutaJson;
        }
    }

    [RelayCommand]
    public async Task GuardarCambiosAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{errores}");
            return;
        }

        if (RutaSeleccionada == null)
        {
            return;
        }

        RutaSeleccionada.IdVehiculo = IdVehiculo;
        RutaSeleccionada.DiasDeRecoleccion = DiasDeRecoleccion!;
        RutaSeleccionada.EstadoRuta = EstadoRuta;
        RutaSeleccionada.PuntosRutaJson = PuntosRutaJson;

        await _rutaRepository.UpdateRutaAsync(RutaSeleccionada);
        await _alertaHelper.ShowSuccessAsync("Ruta actualizada correctamente.");
        await Shell.Current.GoToAsync("..");
    }

    partial void OnIdVehiculoChanged(int value)
    {
        ValidateProperty(value, nameof(IdVehiculo));
        OnPropertyChanged(nameof(IdVehiculoError));
        OnPropertyChanged(nameof(HasIdVehiculoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnDiasDeRecoleccionChanged(string? value)
    {
        ValidateProperty(value, nameof(DiasDeRecoleccion));
        OnPropertyChanged(nameof(DiasDeRecoleccionError));
        OnPropertyChanged(nameof(HasDiasDeRecoleccionError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    public string? IdVehiculoError => GetErrors(nameof(IdVehiculo)).FirstOrDefault()?.ErrorMessage;
    public string? DiasDeRecoleccionError => GetErrors(nameof(DiasDeRecoleccion)).FirstOrDefault()?.ErrorMessage;

    public bool HasIdVehiculoError => GetErrors(nameof(IdVehiculo)).Any();
    public bool HasDiasDeRecoleccionError => GetErrors(nameof(DiasDeRecoleccion)).Any();
    public bool PuedeGuardar => !HasErrors && IdVehiculo > 0 && !string.IsNullOrWhiteSpace(DiasDeRecoleccion);
}
