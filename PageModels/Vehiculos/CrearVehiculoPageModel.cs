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
    public string? placaVehiculo;

    [ObservableProperty]
    [Required(ErrorMessage = "La marca es obligatoria.")]
    public string? marcaVehiculo;

    [ObservableProperty]
    [Required(ErrorMessage = "El modelo es obligatorio.")]
    public string? modeloVehiculo;

    [ObservableProperty]
    public bool estadoVehiculo = true;

    [ObservableProperty]
    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    private Usuario? usuarioSeleccionado;

    [ObservableProperty]
    private DateTime fechaRegistro = DateTime.Now;

    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAlertaHelper _alertaHelper;

    public ObservableCollection<Vehiculo> ListaVehiculo { get; set; } = new();
    public ObservableCollection<Usuario> ListaUsuario { get; set; } = new();


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
    public async Task CargarVehiculosAsync()
    {
        ListaVehiculo.Clear();
        var vehiculos = await _vehiculoRepository.GetAllVehiculoAsync();
        foreach (var vehiculo in vehiculos)
        {
            ListaVehiculo.Add(vehiculo);
        }
    }


    [RelayCommand]
    public async Task CrearVehiculoAsync()
    {

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
}
