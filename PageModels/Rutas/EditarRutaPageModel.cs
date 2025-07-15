using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MauiFirebase.PageModels.Rutas;

[QueryProperty(nameof(IdRuta), "id")]
public partial class EditarRutaPageModel : ObservableValidator
{
    private readonly IRutaRepository _rutaRepository;
    private readonly IAlertaHelper _alertaHelper;
    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly SincronizacionFirebaseService _sincronizar;

    [ObservableProperty]
    private string? _idRuta;

    [ObservableProperty]
    private Ruta? _rutaSeleccionada;

    public ObservableCollection<Vehiculo> ListaVehiculos { get; } = new();

    [ObservableProperty]
    private Vehiculo? _vehiculoSeleccionado;

    public ObservableCollection<string> DiasOpciones { get; } = new()
    {
        "Lunes", "Martes", "Miércoles", "Jueves", "Viernes"
    };

    [ObservableProperty]
    [Required(ErrorMessage = "Debes ingresar el día de recolección.")]
    private string? _diasDeRecoleccion;

    [ObservableProperty]
    private bool _estadoRuta;

    [ObservableProperty]
    private string? _puntosRutaJson;

    public EditarRutaPageModel(IRutaRepository rutaRepository, 
        IAlertaHelper alertaHelper,
        SincronizacionFirebaseService sincronizar,
        IVehiculoRepository vehiculoRepository)
    {
        _rutaRepository = rutaRepository;
        _alertaHelper = alertaHelper;
        _sincronizar = sincronizar;
        _vehiculoRepository = vehiculoRepository;
    }

    public async Task InicializarAsync()
    {
        var vehiculos = await _vehiculoRepository.GetAllVehiculoAsync();
        ListaVehiculos.Clear();
        foreach (var v in vehiculos)
            ListaVehiculos.Add(v);

        RutaSeleccionada = await _rutaRepository.GetRutaIdAsync(IdRuta);
        if (RutaSeleccionada != null)
        {
            VehiculoSeleccionado = ListaVehiculos.FirstOrDefault(v => v.IdVehiculo == RutaSeleccionada.IdVehiculo);
            DiasDeRecoleccion = RutaSeleccionada.DiasDeRecoleccion;
            EstadoRuta = RutaSeleccionada.EstadoRuta;
            PuntosRutaJson = RutaSeleccionada.PuntosRutaJson;
        }
    }

    [RelayCommand]
    public async Task GuardarCambiosAsync()
    {
        ValidateAllProperties();
        if (HasErrors || VehiculoSeleccionado == null)
        {
            var errores = GetErrors().Select(e => e.ErrorMessage).ToList();
            if (VehiculoSeleccionado == null)
                errores.Add("Debes seleccionar un vehículo.");

            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{string.Join("\n", errores)}");
            return;
        }

        if (RutaSeleccionada == null) return;

        RutaSeleccionada.IdVehiculo = VehiculoSeleccionado.IdVehiculo;
        RutaSeleccionada.DiasDeRecoleccion = DiasDeRecoleccion!;
        RutaSeleccionada.EstadoRuta = EstadoRuta;
        RutaSeleccionada.Sincronizado = false;
        RutaSeleccionada.PuntosRutaJson = PuntosRutaJson;

        await _rutaRepository.UpdateRutaAsync(RutaSeleccionada);
        await _alertaHelper.ShowSuccessAsync("Ruta actualizada correctamente.");
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet && _sincronizar is not null)
        {
            try
            {
                await _sincronizar.SincronizarRutasAsync();
            }
            catch
            {
                await _alertaHelper.ShowWarningAsync("Cambios guardados localmente. Se sincronizarán cuando haya conexión.");
            }
        }
        await Shell.Current.GoToAsync("..");
    }

    partial void OnVehiculoSeleccionadoChanged(Vehiculo? value)
    {
        OnPropertyChanged(nameof(HasVehiculoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnDiasDeRecoleccionChanged(string? value)
    {
        ValidateProperty(value, nameof(DiasDeRecoleccion));
        OnPropertyChanged(nameof(DiasDeRecoleccionError));
        OnPropertyChanged(nameof(HasDiasDeRecoleccionError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    public bool HasVehiculoError => VehiculoSeleccionado == null;
    public string? DiasDeRecoleccionError => GetErrors(nameof(DiasDeRecoleccion)).FirstOrDefault()?.ErrorMessage;
    public bool HasDiasDeRecoleccionError => GetErrors(nameof(DiasDeRecoleccion)).Any();

    public bool PuedeGuardar =>
        !HasErrors &&
        VehiculoSeleccionado != null &&
        !string.IsNullOrWhiteSpace(DiasDeRecoleccion);
}
