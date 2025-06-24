using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
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

    public CrearPremioPageModel(IPremioRepository premioRepository)
    {
        _premioRepository = premioRepository;
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
        await AppShell.DisplayToastAsync("Premio guardado correctamente");
        await Shell.Current.GoToAsync(".."); // Volver a la página anterior
    }
}
