using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Pages.Premio;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Premios;

public partial class PremioPageModel : ObservableObject
{
    public ObservableCollection<Premio> ListaPremios { get; } = new();

    private readonly IPremioRepository _premioRepository;

    public PremioPageModel(IPremioRepository premioRepository)
    {
        _premioRepository = premioRepository;
    }

    [RelayCommand]
    public async Task CargarPremiosAsync()
    {
        ListaPremios.Clear(); // Limpiamos primero
        var premios = await _premioRepository.GetAllPremiosAsync(); // Obtenemos desde BD
        foreach (var p in premios)
        {
            ListaPremios.Add(p); // Añadimos a la ObservableCollection
        }
    }


    [RelayCommand]
    public async Task CambiarEstadoPremioAsync(int id)
    {
        await _premioRepository.ChangePremioStatusAsync(id);
        await CargarPremiosAsync();
    }

    [RelayCommand]
    public async Task IrAEditarPremioAsync(Premio premio)
    {
        var parametros = new Dictionary<string, object>
        {
            { "id", premio.IdPremio }
        };

        await Shell.Current.GoToAsync($"{nameof(EditarPremioPage)}?id={premio.IdPremio}");
    }

    [RelayCommand]
    public async Task IrACrearPremioAsync()
    {
        await Shell.Current.GoToAsync(nameof(AgregarPremioPage));
    }
}
