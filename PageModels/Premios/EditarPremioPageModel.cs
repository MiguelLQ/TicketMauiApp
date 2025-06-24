using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Linq;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Premios;

[QueryProperty(nameof(IdPremio), "id")]
public partial class EditarPremioPageModel : ObservableObject
{
    private readonly IPremioRepository _premioRepository;

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

    public EditarPremioPageModel(IPremioRepository premioRepository)
    {
        _premioRepository = premioRepository;
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

        await _premioRepository.UpdatePremioAsync(PremioSeleccionado);
        await Shell.Current.GoToAsync("..");
    }
}
