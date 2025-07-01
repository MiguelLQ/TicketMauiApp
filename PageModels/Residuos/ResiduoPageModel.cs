using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
namespace MauiFirebase.PageModels.Residuos;
public partial class ResiduoPageModel : ObservableObject
{
    public ObservableCollection<Residuo> ListaResiduos { get; } = new();
    public ObservableCollection<CategoriaResiduo> ListaCategorias { get; } = new();

    private readonly IResiduoRepository _residuoRepository;
    private readonly ICategoriaResiduoRepository _categoriaResiduoRepository;
    private readonly IAlertaHelper _alertaHelper;

    [ObservableProperty]
    private bool isBusy;

    public ResiduoPageModel(IResiduoRepository residuoRepository, ICategoriaResiduoRepository categoriaResiduoRepository, IAlertaHelper alertaHelper)
    {
        _residuoRepository = residuoRepository;
        _categoriaResiduoRepository = categoriaResiduoRepository;
        _alertaHelper = alertaHelper;
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
        try
        {
            IsBusy = true;
            //await Task.Delay(800);
            ListaResiduos.Clear();
            var residuos = await _residuoRepository.GetAllResiduoAync();
            var categorias = await _categoriaResiduoRepository.GetAllCategoriaResiduoAsync();

            foreach (var residuo in residuos)
            {
                var categoria = categorias.FirstOrDefault(c => c.IdCategoriaResiduo == residuo.IdCategoriaResiduo);
                residuo.NombreCategoria = categoria?.NombreCategoria;
                ListaResiduos.Add(residuo);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
    public async Task CambiarEstadoResiduoAsync(int id)
    {
        await _residuoRepository.ChangeEstadoResiduoAsync(id);
        await _alertaHelper.ShowSuccessAsync("Se cambio de estado de manera exitosa");
        await CargarResiduosAsync();
    }


    [RelayCommand]
    public async Task IrACrearResiduoAsync()
    {
        await Shell.Current.GoToAsync("AgregarResiduoPage");
    }
}
