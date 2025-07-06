using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Helpers.Interface;
using System.ComponentModel.DataAnnotations;

namespace MauiFirebase.PageModels.Vehiculos;

[QueryProperty(nameof(IdVehiculo), "id")]
public partial class EditarVehiculoPageModel : ObservableValidator
{
    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly IAlertaHelper _alertaHelper;

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
    private string? idUsuario; // Suponiendo que es un string (Uid)

    public EditarVehiculoPageModel(
        IVehiculoRepository vehiculoRepository,
        IAlertaHelper alertaHelper)
    {
        _vehiculoRepository = vehiculoRepository;
        _alertaHelper = alertaHelper;
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
            IdUsuario = vehiculo.IdUsuario;
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
}
