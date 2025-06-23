using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
namespace MauiFirebase.PageModels.Residuos;
public partial class ResiduoPageModel : ObservableObject
{
    public ObservableCollection<Residuo> ListaResiduos { get; } = new();
    public ObservableCollection<CategoriaResiduo> ListaCategorias { get; } = new();

    private readonly IResiduoRepository _residuoRepository;
    private readonly ICategoriaResiduoRepository _categoriaResiduoRepository;

    public ResiduoPageModel(IResiduoRepository residuoRepository, ICategoriaResiduoRepository categoriaResiduoRepository)
    {
        _residuoRepository = residuoRepository;
        _categoriaResiduoRepository = categoriaResiduoRepository;
    }

    [RelayCommand]
    public async Task CargarCategoriaResiduoAsync()
    {
        ListaCategorias.Clear();
        var categorias = await _categoriaResiduoRepository.GetAllCategoriaResiduoAsync();
        foreach (var c in categorias)
        {
            ListaCategorias.Add(c);
        }
    }

    [RelayCommand]
    public async Task CargarResiduosAsync()
    {
        ListaResiduos.Clear();
        var residuos = await _residuoRepository.GetAllResiduoAync();
        foreach (var r in residuos)
        {
            ListaResiduos.Add(r);
        }
    }

    [RelayCommand]
    public async Task CambiarEstadoResiduoAsync(int id)
    {
        await _residuoRepository.ChangeEstadoResiduoAsync(id);
        await CargarResiduosAsync();
    }


    [RelayCommand]
    public async Task IrACrearResiduoAsync()
    {
        await Shell.Current.GoToAsync("AgregarResiduoPage");
    }

}
