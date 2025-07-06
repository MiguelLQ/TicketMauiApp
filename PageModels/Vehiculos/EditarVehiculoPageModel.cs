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

    [ObservableProperty]
    private int idVehiculo;

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
    private bool estadoVehiculo;

    [ObservableProperty]
    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    private string? idUsuario;
    [ObservableProperty]
    private ObservableCollection<Usuario> listaUsuario = new();

    [ObservableProperty]
    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    private Usuario? usuarioSeleccionado;


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
        if (vehiculo != null)
        {
            PlacaVehiculo = vehiculo.PlacaVehiculo;
            MarcaVehiculo = vehiculo.MarcaVehiculo;
            ModeloVehiculo = vehiculo.ModeloVehiculo;
            EstadoVehiculo = vehiculo.EstadoVehiculo;
            var usuarios = await _usuarioRepositorio.GetUsuariosAsync();
            ListaUsuario = new ObservableCollection<Usuario>(usuarios);
            UsuarioSeleccionado = ListaUsuario.FirstOrDefault(u => u.Uid == vehiculo.IdUsuario);
        }
    }

    [RelayCommand]
    public async Task EditarVehiculoAsync()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{errores}");
            return;
        }

        var actualizado = new Vehiculo
        {
            IdVehiculo = IdVehiculo,
            PlacaVehiculo = PlacaVehiculo!,
            MarcaVehiculo = MarcaVehiculo!,
            ModeloVehiculo = ModeloVehiculo!,
            EstadoVehiculo = EstadoVehiculo,
            IdUsuario = IdUsuario!,
            FechaRegistroVehiculo = DateTime.Now
        };

        await _vehiculoRepository.UpdateVehiculoAsync(actualizado);
        await _alertaHelper.ShowSuccessAsync("Vehículo actualizado correctamente.");
        await Shell.Current.GoToAsync("..");
    }

    /* ===================================================================================
     * VALIDACIONES EN TIEMPO REAL (XAML Binding Friendly)
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

    partial void OnIdUsuarioChanged(string? value)
    {
        ValidateProperty(value, nameof(IdUsuario));
        OnPropertyChanged(nameof(IdUsuarioError));
        OnPropertyChanged(nameof(HasIdUsuarioError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    /* ===================================================================================
     * ERRORES DE PROPIEDADES PARA USO EN XAML
    =================================================================================== */

    public string? PlacaVehiculoError => GetErrors(nameof(PlacaVehiculo)).FirstOrDefault()?.ErrorMessage;
    public string? MarcaVehiculoError => GetErrors(nameof(MarcaVehiculo)).FirstOrDefault()?.ErrorMessage;
    public string? ModeloVehiculoError => GetErrors(nameof(ModeloVehiculo)).FirstOrDefault()?.ErrorMessage;
    public string? IdUsuarioError => GetErrors(nameof(IdUsuario)).FirstOrDefault()?.ErrorMessage;

    public bool HasPlacaVehiculoError => GetErrors(nameof(PlacaVehiculo)).Any();
    public bool HasMarcaVehiculoError => GetErrors(nameof(MarcaVehiculo)).Any();
    public bool HasModeloVehiculoError => GetErrors(nameof(ModeloVehiculo)).Any();
    public bool HasIdUsuarioError => GetErrors(nameof(IdUsuario)).Any();

    public bool PuedeGuardar =>
        !HasErrors &&
        !string.IsNullOrWhiteSpace(PlacaVehiculo) &&
        !string.IsNullOrWhiteSpace(MarcaVehiculo) &&
        !string.IsNullOrWhiteSpace(ModeloVehiculo) &&
        !string.IsNullOrWhiteSpace(IdUsuario);
}
