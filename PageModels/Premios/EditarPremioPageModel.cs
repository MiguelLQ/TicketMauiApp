using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace MauiFirebase.PageModels.Premios;

[QueryProperty(nameof(IdPremio), "id")]
public partial class EditarPremioPageModel : ObservableObject
{
    private readonly IPremioRepository _premioRepository;
    private readonly IAlertaHelper _alertaHelper;

    [ObservableProperty]
    private int idPremio;

    [ObservableProperty]
    private string tituloPagina = "Editar Premio";

    [ObservableProperty]
    private string? nombrePremio;

    [ObservableProperty]
    private string? descripcionPremio;

    [ObservableProperty]
    private int puntosRequeridos;

    [ObservableProperty]
    private bool estadoPremio;

    [ObservableProperty]
    private Premio? premioSeleccionado;

    [ObservableProperty]
    private string? fotoPremio;


    public EditarPremioPageModel(IPremioRepository premioRepository, IAlertaHelper alertaHelper)
    {
        _premioRepository = premioRepository;
        _alertaHelper = alertaHelper;
    }

    public async Task InicializarAsync()
    {
        PremioSeleccionado = await _premioRepository.GetPremioByIdAsync(IdPremio);

        if (PremioSeleccionado != null)
        {
            NombrePremio = PremioSeleccionado.NombrePremio;
            DescripcionPremio = PremioSeleccionado.DescripcionPremio;
            PuntosRequeridos = PremioSeleccionado.PuntosRequeridos;
            EstadoPremio = PremioSeleccionado.EstadoPremio;
            FotoPremio = PremioSeleccionado.FotoPremio ?? string.Empty; // ✅ se carga la foto si existe
        }
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
    public async Task GuardarCambiosAsync()
    {
        if (PremioSeleccionado == null)
            return;

        PremioSeleccionado.NombrePremio = NombrePremio;
        PremioSeleccionado.DescripcionPremio = DescripcionPremio;
        PremioSeleccionado.PuntosRequeridos = PuntosRequeridos;
        PremioSeleccionado.EstadoPremio = EstadoPremio;
        PremioSeleccionado.FotoPremio = FotoPremio ?? string.Empty; // ✅ se guarda la foto

        await _premioRepository.UpdatePremioAsync(PremioSeleccionado);
        await _alertaHelper.ShowSuccessAsync("Premio actualizado correctamente.");
        await Shell.Current.GoToAsync("..");
    }
}
