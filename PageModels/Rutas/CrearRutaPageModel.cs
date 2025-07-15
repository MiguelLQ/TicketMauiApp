using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using MauiFirebase.Pages.Ruta;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MauiFirebase.PageModels.Rutas;

public partial class CrearRutaPageModel : ObservableValidator
{
    public ObservableCollection<Vehiculo> ListaVehiculos { get; } = new();

    [ObservableProperty]
    private Vehiculo? _vehiculoSeleccionado;

    public ObservableCollection<string> DiasOpciones { get; } = new()
    {
        "lunes", "martes", "miércoles", "jueves", "viernes", "sabado", "domingo"
    };

    [ObservableProperty]
    [Required(ErrorMessage = "Debes seleccionar un día de recolección.")]
    private string? _diasDeRecoleccion;

    [ObservableProperty]
    private bool _estadoRuta = true;

    [ObservableProperty]
    private string? _puntosRutaJson;

    private readonly IRutaRepository _rutaRepository;
    private readonly IAlertaHelper _alertaHelper;
    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly SincronizacionFirebaseService _sincronizar;

    public CrearRutaPageModel(IRutaRepository rutaRepository, 
        IAlertaHelper alertaHelper,
        SincronizacionFirebaseService sincronizar,
    IVehiculoRepository vehiculoRepository)
    {
        _rutaRepository = rutaRepository;
        _alertaHelper = alertaHelper;
        _sincronizar = sincronizar;
        _vehiculoRepository = vehiculoRepository;
    }

    [RelayCommand]
    public async Task CrearRutaAsync()
    {
        ValidateAllProperties();

        if (HasErrors || VehiculoSeleccionado == null)
        {
            var errores = GetErrors()
                .Select(e => e.ErrorMessage)
                .Append(VehiculoSeleccionado == null ? "Debes seleccionar un vehículo." : null)
                .Where(e => !string.IsNullOrWhiteSpace(e));

            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{string.Join("\n", errores)}");
            return;
        }

        var nuevaRuta = new Ruta
        {
            IdVehiculo = VehiculoSeleccionado.IdVehiculo,
            DiasDeRecoleccion = DiasDeRecoleccion!,
            EstadoRuta = EstadoRuta,
            PuntosRutaJson = PuntosRutaJson,
            FechaRegistroRuta = DateTime.Now,
            Sincronizado = false
        };

        await _rutaRepository.CreateRutaAsync(nuevaRuta);
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            try
            {
                await _sincronizar.SincronizarRutasAsync();
            }
            catch
            {
                await _alertaHelper.ShowWarningAsync("Guardado localmente. Se sincronizará cuando haya internet.");
            }
        }
        await _alertaHelper.ShowSuccessAsync("Ruta creada correctamente.");
        LimpiarFormulario();
        await Shell.Current.GoToAsync(nameof(ListarRutaPage));
    }

    [RelayCommand]
    public async Task CargarVehiculosAsync()
    {
        ListaVehiculos.Clear();
        var vehiculos = await _vehiculoRepository.GetAllVehiculoAsync();
        foreach (var v in vehiculos)
            ListaVehiculos.Add(v);
    }

    private void LimpiarFormulario()
    {
        VehiculoSeleccionado = null;
        DiasDeRecoleccion = null;
        PuntosRutaJson = null;
        EstadoRuta = true;
        ClearErrors();
    }
    partial void OnDiasDeRecoleccionChanged(string? value)
    {
        ValidateProperty(value, nameof(DiasDeRecoleccion));
        OnPropertyChanged(nameof(DiasDeRecoleccionError));
        OnPropertyChanged(nameof(HasDiasDeRecoleccionError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    public string? DiasDeRecoleccionError => GetErrors(nameof(DiasDeRecoleccion)).FirstOrDefault()?.ErrorMessage;
    public bool HasDiasDeRecoleccionError => GetErrors(nameof(DiasDeRecoleccion)).Any();

    public bool PuedeGuardar =>
        !HasErrors &&
        VehiculoSeleccionado != null &&
        !string.IsNullOrWhiteSpace(DiasDeRecoleccion);
}
