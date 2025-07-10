using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Rutas;

public partial class CrearRutaPageModel : ObservableValidator
{
    public ObservableCollection<Vehiculo> ListaVehiculos { get; } = new();

    [ObservableProperty]
    private Vehiculo? _vehiculoSeleccionado;

    public ObservableCollection<string> DiasOpciones { get; } = new()
{
    "Lunes", "Martes", "Miércoles", "Jueves", "Viernes"
};

    [ObservableProperty]
    private string? _diasDeRecoleccion;

    [ObservableProperty]
    private bool _estadoRuta = true;

    [ObservableProperty]
    private string? _puntosRutaJson;

    private readonly IRutaRepository _rutaRepository;
    private readonly IAlertaHelper _alertaHelper;
    private readonly IVehiculoRepository _vehiculoRepository;

    public CrearRutaPageModel(IRutaRepository rutaRepository, IAlertaHelper alertaHelper, IVehiculoRepository vehiculoRepository)
    {
        _rutaRepository = rutaRepository;
        _alertaHelper = alertaHelper;
        _vehiculoRepository = vehiculoRepository;
    }

    [RelayCommand]
    public async Task CrearRutaAsync()
    {
        ValidateAllProperties();

        if (HasErrors || VehiculoSeleccionado == null)
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            if (VehiculoSeleccionado == null)
                errores += "\nDebes seleccionar un vehículo.";
            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{errores}");
            return;
        }

        var nueva = new Ruta
        {
            IdVehiculo = VehiculoSeleccionado.IdVehiculo,
            DiasDeRecoleccion = DiasDeRecoleccion!,
            EstadoRuta = EstadoRuta,
            PuntosRutaJson = PuntosRutaJson,
            FechaRegistroRuta = DateTime.Now
        };

        await _rutaRepository.CreateRutaAsync(nueva);
        await _alertaHelper.ShowSuccessAsync("Ruta creada correctamente.");
        LimpiarFormulario();
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task CargarVehiculosAsync()
    {
        ListaVehiculos.Clear();
        var vehiculos = await _vehiculoRepository.GetAllVehiculoAsync();
        foreach (var v in vehiculos)
        {
            ListaVehiculos.Add(v);
        }
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
