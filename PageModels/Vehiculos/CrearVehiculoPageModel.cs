using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Helpers.Interface;
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Vehiculos;

public partial class CrearVehiculoPageModel : ObservableValidator
{
    [ObservableProperty]
    [Required(ErrorMessage = "La placa es obligatoria.")]
    private string? placaVehiculo;

    [ObservableProperty]
    [Required(ErrorMessage = "La marca es obligatoria.")]
    private string? marcaVehiculo;

    [ObservableProperty]
    [Required(ErrorMessage = "El modelo es obligatorio.")]
    private string? modeloVehiculo;

    [ObservableProperty]
    private bool estadoVehiculo = true;

    [ObservableProperty]
    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    private Usuario? usuarioSeleccionado;

    [ObservableProperty]
    private DateTime fechaRegistro = DateTime.Now;

    public ObservableCollection<Usuario> ListaUsuario { get; } = new();

    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAlertaHelper _alertaHelper;

    public CrearVehiculoPageModel(
        IVehiculoRepository vehiculoRepository,
        IUsuarioRepository usuarioRepository,
        IAlertaHelper alertaHelper)
    {
        _vehiculoRepository = vehiculoRepository;
        _usuarioRepository = usuarioRepository;
        _alertaHelper = alertaHelper;
    }

    [RelayCommand]
    public async Task CargarUsuariosAsync()
    {
        ListaUsuario.Clear();
        var usuarios = await _usuarioRepository.GetUsuariosAsync();
        foreach (var usuario in usuarios)
        {
            ListaUsuario.Add(usuario);
        }
    }

    [RelayCommand]
    public async Task CrearVehiculoAsync()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{errores}");
            return;
        }

        var nuevoVehiculo = new Vehiculo
        {
            PlacaVehiculo = PlacaVehiculo!,
            MarcaVehiculo = MarcaVehiculo!,
            ModeloVehiculo = ModeloVehiculo!,
            EstadoVehiculo = EstadoVehiculo,
            FechaRegistroVehiculo = FechaRegistro,
            IdUsuario = UsuarioSeleccionado?.Uid,
            Nombre = UsuarioSeleccionado?.Nombre ?? string.Empty
        };

        await _vehiculoRepository.CreateVehiculoAsync(nuevoVehiculo);
        await _alertaHelper.ShowSuccessAsync("Vehículo creado correctamente.");
        LimpiarFormulario();
        await Shell.Current.GoToAsync("..");
    }

    public void LimpiarFormulario()
    {
        PlacaVehiculo = string.Empty;
        MarcaVehiculo = string.Empty;
        ModeloVehiculo = string.Empty;
        EstadoVehiculo = true;
        UsuarioSeleccionado = null;
        ClearErrors();
    }

    /* ===================================================================================
     * VALIDACIONES EN TIEMPO REAL
    =================================================================================== */

    partial void OnPlacaVehiculoChanged(string? value)
    {
        ValidateProperty(value, nameof(PlacaVehiculo));
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
        OnPropertyChanged(nameof(UsuarioError));
        OnPropertyChanged(nameof(HasUsuarioError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    /* ===================================================================================
     * PROPIEDADES DE ERROR PARA EL XAML
    =================================================================================== */

    public string? PlacaVehiculoError => GetErrors(nameof(PlacaVehiculo)).FirstOrDefault()?.ErrorMessage;
    public string? MarcaVehiculoError => GetErrors(nameof(MarcaVehiculo)).FirstOrDefault()?.ErrorMessage;
    public string? ModeloVehiculoError => GetErrors(nameof(ModeloVehiculo)).FirstOrDefault()?.ErrorMessage;
    public string? UsuarioError => GetErrors(nameof(UsuarioSeleccionado)).FirstOrDefault()?.ErrorMessage;

    public bool HasPlacaVehiculoError => GetErrors(nameof(PlacaVehiculo)).Any();
    public bool HasMarcaVehiculoError => GetErrors(nameof(MarcaVehiculo)).Any();
    public bool HasModeloVehiculoError => GetErrors(nameof(ModeloVehiculo)).Any();
    public bool HasUsuarioError => GetErrors(nameof(UsuarioSeleccionado)).Any();

    public bool PuedeGuardar =>
        !HasErrors &&
        !string.IsNullOrWhiteSpace(PlacaVehiculo) &&
        !string.IsNullOrWhiteSpace(MarcaVehiculo) &&
        !string.IsNullOrWhiteSpace(ModeloVehiculo) &&
        UsuarioSeleccionado != null;
}
