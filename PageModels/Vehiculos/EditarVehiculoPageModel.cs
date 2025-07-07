using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MauiFirebase.PageModels.Vehiculos;

[QueryProperty(nameof(IdVehiculo), "id")]
public partial class EditarVehiculoPageModel : ObservableValidator
{
    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly IAlertaHelper _alertaHelper;
    private readonly IUsuarioRepository _usuarioRepositorio;

    public ObservableCollection<Usuario> ListaUsuario { get; } = new();

    [ObservableProperty]
    private int idVehiculo;

    [ObservableProperty]
    [Required(ErrorMessage = "La placa es obligatoria.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "La placa debe tener exactamente 6 caracteres.")]
    private string? placaVehiculo;

    [ObservableProperty]
    [Required(ErrorMessage = "La marca es obligatoria.")]
    private string? marcaVehiculo;

    [ObservableProperty]
    [Required(ErrorMessage = "El modelo es obligatorio.")]
    private string? modeloVehiculo;

    [ObservableProperty]
    private bool estadoVehiculo;

    [ObservableProperty]
    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    private Usuario? usuarioSeleccionado;

    [ObservableProperty]
    private string? placaDuplicadaError;

    public bool HasPlacaDuplicadaError => !string.IsNullOrWhiteSpace(PlacaDuplicadaError);

    public EditarVehiculoPageModel(
        IVehiculoRepository vehiculoRepository,
        IAlertaHelper alertaHelper,
        IUsuarioRepository usuarioRepositorio)
    {
        _vehiculoRepository = vehiculoRepository;
        _alertaHelper = alertaHelper;
        _usuarioRepositorio = usuarioRepositorio;
    }

    public async Task InicializarAsync()
    {
        var vehiculo = await _vehiculoRepository.GetVehiculoByIdAsync(IdVehiculo);
        var usuarios = await _usuarioRepositorio.GetUsuariosAsync();

        var conductores = usuarios.Where(u => u.Rol?.ToLower() == "conductor").ToList();
        foreach (var usuario in conductores)
            ListaUsuario.Add(usuario);

        if (vehiculo != null)
        {
            PlacaVehiculo = vehiculo.PlacaVehiculo;
            MarcaVehiculo = vehiculo.MarcaVehiculo;
            ModeloVehiculo = vehiculo.ModeloVehiculo;
            EstadoVehiculo = vehiculo.EstadoVehiculo;
            UsuarioSeleccionado = ListaUsuario.FirstOrDefault(u => u.Uid == vehiculo.IdUsuario);
        }
    }

    [RelayCommand]
    public async Task GuardarCambiosAsync()
    {
        ValidateAllProperties();

        if (HasErrors || HasPlacaDuplicadaError)
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            if (HasPlacaDuplicadaError)
                errores += $"\n{PlacaDuplicadaError}";

            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{errores}");
            return;
        }

        var vehiculo = new Vehiculo
        {
            IdVehiculo = IdVehiculo,
            PlacaVehiculo = PlacaVehiculo!,
            MarcaVehiculo = MarcaVehiculo!,
            ModeloVehiculo = ModeloVehiculo!,
            EstadoVehiculo = EstadoVehiculo,
            IdUsuario = UsuarioSeleccionado!.Uid,
            FechaRegistroVehiculo = DateTime.Now
        };

        await _vehiculoRepository.UpdateVehiculoAsync(vehiculo);
        await _alertaHelper.ShowSuccessAsync("Vehículo actualizado correctamente.");
        await Shell.Current.GoToAsync("..");
    }

    /* =============================================
     * VALIDACIONES EN TIEMPO REAL
    ============================================= */

    partial void OnPlacaVehiculoChanged(string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            PlacaVehiculo = value.ToUpper();

        ValidateProperty(PlacaVehiculo, nameof(PlacaVehiculo));
        _ = VerificarPlacaDuplicadaAsync(PlacaVehiculo);

        OnPropertyChanged(nameof(PlacaVehiculoError));
        OnPropertyChanged(nameof(HasPlacaVehiculoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnMarcaVehiculoChanged(string? value)
    {
        ValidateProperty(value, nameof(MarcaVehiculo));
        OnPropertyChanged(nameof(MarcaVehiculoError));
        OnPropertyChanged(nameof(HasMarcaVehiculoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnModeloVehiculoChanged(string? value)
    {
        ValidateProperty(value, nameof(ModeloVehiculo));
        OnPropertyChanged(nameof(ModeloVehiculoError));
        OnPropertyChanged(nameof(HasModeloVehiculoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnUsuarioSeleccionadoChanged(Usuario? value)
    {
        ValidateProperty(value, nameof(UsuarioSeleccionado));
        OnPropertyChanged(nameof(UsuarioSeleccionadoError));
        OnPropertyChanged(nameof(HasUsuarioSeleccionadoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    private async Task VerificarPlacaDuplicadaAsync(string? placa)
    {
        PlacaDuplicadaError = null;

        if (!string.IsNullOrWhiteSpace(placa) && placa.Length == 6)
        {
            var existente = await _vehiculoRepository.GetVehiculoPorPlacaAsync(placa);
            if (existente != null && existente.IdVehiculo != IdVehiculo)
                PlacaDuplicadaError = "Ya existe un vehículo registrado con esta placa.";
        }

        OnPropertyChanged(nameof(PlacaDuplicadaError));
        OnPropertyChanged(nameof(HasPlacaDuplicadaError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    /* =============================================
     * ERRORES PARA EL XAML
    ============================================= */

    public string? PlacaVehiculoError => GetErrors(nameof(PlacaVehiculo)).FirstOrDefault()?.ErrorMessage;
    public string? MarcaVehiculoError => GetErrors(nameof(MarcaVehiculo)).FirstOrDefault()?.ErrorMessage;
    public string? ModeloVehiculoError => GetErrors(nameof(ModeloVehiculo)).FirstOrDefault()?.ErrorMessage;
    public string? UsuarioSeleccionadoError => GetErrors(nameof(UsuarioSeleccionado)).FirstOrDefault()?.ErrorMessage;

    public bool HasPlacaVehiculoError => GetErrors(nameof(PlacaVehiculo)).Any();
    public bool HasMarcaVehiculoError => GetErrors(nameof(MarcaVehiculo)).Any();
    public bool HasModeloVehiculoError => GetErrors(nameof(ModeloVehiculo)).Any();
    public bool HasUsuarioSeleccionadoError => GetErrors(nameof(UsuarioSeleccionado)).Any();

    public bool PuedeGuardar =>
        !HasErrors &&
        !HasPlacaDuplicadaError &&
        !string.IsNullOrWhiteSpace(PlacaVehiculo) &&
        !string.IsNullOrWhiteSpace(MarcaVehiculo) &&
        !string.IsNullOrWhiteSpace(ModeloVehiculo) &&
        UsuarioSeleccionado != null;
}
