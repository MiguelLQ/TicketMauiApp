using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;

namespace MauiFirebase.PageModels.Premios;

public partial class CrearPremioPageModel : ObservableObject
{
    [ObservableProperty]
    private string? nombrePremio;

    [ObservableProperty]
    private string? descripcionPremio;

    [ObservableProperty]
    private int puntosRequeridos;

    [ObservableProperty]
    private bool estadoPremio = true;

    private readonly IPremioRepository _premioRepository;
    private readonly IAlertaHelper _alertaHelper;

    public CrearPremioPageModel(IPremioRepository premioRepository, IAlertaHelper alertaHelper)
    {
        _premioRepository = premioRepository;
        _alertaHelper = alertaHelper;
    }

    [RelayCommand]
    public async Task AddPremioAsync()
    {
        var nuevo = new Premio
        {
            NombrePremio = NombrePremio,
            DescripcionPremio = DescripcionPremio,
            PuntosRequeridos = PuntosRequeridos,
            EstadoPremio = EstadoPremio
        };

        await _premioRepository.CreatePremioAsync(nuevo);
        await _alertaHelper.ShowSuccessAsync("Premio creado correctamente.");
        await Shell.Current.GoToAsync(".."); // Volver a la página anterior
    }
}
