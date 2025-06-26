using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Residuos;

public partial class CrearResiduoPageModel : ObservableObject
{
    [ObservableProperty]
    private string? _nombreResiduo;

    [ObservableProperty]
    private bool _estadoResiduo = true;

    [ObservableProperty]
    private CategoriaResiduo? _categoriaResiduoSeleccionada;

    public ObservableCollection<CategoriaResiduo> ListaCategorias { get; } = new();
        
    private readonly IResiduoRepository _residuoRepository;
    private readonly ICategoriaResiduoRepository _categoriaResiduoRepository;
    private readonly IAlertaHelper _alertaHelper;

    public CrearResiduoPageModel(IResiduoRepository residuoRepository, ICategoriaResiduoRepository categoriaResiduoRepository, IAlertaHelper alertaHelper)
    {
        _residuoRepository = residuoRepository;
        _categoriaResiduoRepository = categoriaResiduoRepository;
        _alertaHelper = alertaHelper;
    }

    [RelayCommand]
    public async Task CargarCategoriasAsync()
    {
        ListaCategorias.Clear();
        var categorias = await _categoriaResiduoRepository.GetAllCategoriaResiduoAsync();
        foreach (var c in categorias)
        {
            ListaCategorias.Add(c);
        }
    }


    [RelayCommand]
    public async Task CrearResiduoAsync()
    {
        var nuevo = new Residuo
        {
            NombreResiduo = NombreResiduo,
            EstadoResiduo = EstadoResiduo,
            IdCategoriaResiduo = CategoriaResiduoSeleccionada?.IdCategoriaResiduo ?? 0
        };

        await _residuoRepository.CreateResiduoAsync(nuevo);
        await _alertaHelper.ShowSuccessAsync("Residuo creado correctamente.");
        await Shell.Current.GoToAsync(".."); // Regresar al listado
    }
}
