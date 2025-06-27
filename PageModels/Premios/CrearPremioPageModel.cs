using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using Microsoft.Maui.Storage;
using System.Threading.Tasks;

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

    [ObservableProperty]
    private string? fotoPremio;

    private readonly IPremioRepository _premioRepository;
    private readonly IAlertaHelper _alertaHelper;

    public int IdPremio { get; private set; }

    public CrearPremioPageModel(IPremioRepository premioRepository, IAlertaHelper alertaHelper)
    {
        _premioRepository = premioRepository;
        _alertaHelper = alertaHelper;
    }

    [RelayCommand]
    public async Task SeleccionarImagenAsync()
    {
        var resultado = await FilePicker.PickAsync(new PickOptions
        {
            FileTypes = FilePickerFileType.Images,
            PickerTitle = "Selecciona una imagen"
        });

        if (resultado != null)
        {
            FotoPremio = resultado.FullPath;
        }
    }

    [RelayCommand]
    public async Task AddPremioAsync()
    {
        var nuevo = new Premio
        {
            NombrePremio = NombrePremio,
            DescripcionPremio = DescripcionPremio,
            PuntosRequeridos = PuntosRequeridos,
            EstadoPremio = EstadoPremio,
            FotoPremio = FotoPremio // ✅ IMPORTANTE
        };

        await _premioRepository.CreatePremioAsync(nuevo);

        await _alertaHelper.ShowSuccessAsync("Premio creado correctamente.");
        await Shell.Current.GoToAsync(".."); // Volver a la lista
    }
    private void LimpiarFormulario()
    {
        IdPremio = 0; // Para indicar que es un nuevo registro
        NombrePremio = string.Empty;
        DescripcionPremio = string.Empty;
        PuntosRequeridos = 0; // int se limpia con cero
        EstadoPremio = false; // Asume que es bool. Ajusta si es string.
        FotoPremio = string.Empty;
    }


}
