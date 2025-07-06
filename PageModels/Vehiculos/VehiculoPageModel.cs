using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Helpers.Interface;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Vehiculos;

public partial class VehiculoPageModel : ObservableObject
{
    public ObservableCollection<Vehiculo> ListaVehiculos { get; } = new();
    private readonly IVehiculoRepository _vehiculoRepository;
    private readonly IAlertaHelper _alertaHelper;

    [ObservableProperty]
    private bool isBusy;

    public VehiculoPageModel(IVehiculoRepository vehiculoRepository, IAlertaHelper alertaHelper)
    {
        _vehiculoRepository = vehiculoRepository;
        _alertaHelper = alertaHelper;
    }

    [RelayCommand]
    public async Task CargarVehiculosAsync()
    {
        try
        {
            IsBusy = true;
            ListaVehiculos.Clear();
            var vehiculos = await _vehiculoRepository.GetAllVehiculoAsync();
            foreach (var vehiculo in vehiculos)
            {
                ListaVehiculos.Add(vehiculo);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CambiarEstadoVehiculoAsync(int id)
    {
        await _vehiculoRepository.ChangeEstadoVehiculoAsync(id);
        await _alertaHelper.ShowSuccessAsync("Se cambió el estado del vehículo correctamente");
        await CargarVehiculosAsync();
    }

    [RelayCommand]
    public async Task IrACrearVehiculoAsync()
    {
        await Shell.Current.GoToAsync("AgregarVehiculoPage");
    }
}
