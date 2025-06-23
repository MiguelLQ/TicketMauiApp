using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Residuos;
[QueryProperty(nameof(IdResiduo), "id")]
public partial class EditarResiduoPageModel : ObservableObject
{
    private readonly IResiduoRepository _residuoRepository;
    private readonly ICategoriaResiduoRepository _categoriaResiduoRepository;

    public ObservableCollection<CategoriaResiduo> ListaCategorias { get; } = new();

    [ObservableProperty]
    private int idResiduo;

    [ObservableProperty]
    private Residuo? _residuoSeleccionado;

    [ObservableProperty]
    private string? _nombreResiduo;

    [ObservableProperty]
    private bool _estadoResiduo;

    [ObservableProperty]
    private CategoriaResiduo? _categoriaResiduoSeleccionada;

    public EditarResiduoPageModel(IResiduoRepository residuoRepository, ICategoriaResiduoRepository categoriaResiduoRepository)
    {
        _residuoRepository = residuoRepository;
        _categoriaResiduoRepository = categoriaResiduoRepository;
    }

    public async Task InicializarAsync()
    {
        await CargarCategoriasAsync();

        ResiduoSeleccionado = await _residuoRepository.GetResiduoIdAsync(IdResiduo);

        if (ResiduoSeleccionado != null)
        {
            NombreResiduo = ResiduoSeleccionado.NombreResiduo;
            EstadoResiduo = ResiduoSeleccionado.EstadoResiduo;
            CategoriaResiduoSeleccionada = ListaCategorias.FirstOrDefault(c => c.IdCategoriaResiduo == ResiduoSeleccionado.IdCategoriaResiduo);
        }
    }

    [RelayCommand]
    public async Task CargarCategoriasAsync()
    {
        ListaCategorias.Clear();
        var categorias = await _categoriaResiduoRepository.GetAllCategoriaResiduoAsync();
        foreach (var categoria in categorias)
        {
            ListaCategorias.Add(categoria);
        }
    }

    [RelayCommand]
    public async Task GuardarCambiosAsync()
    {
        if (ResiduoSeleccionado == null)
            return;

        ResiduoSeleccionado.NombreResiduo = NombreResiduo;
        ResiduoSeleccionado.EstadoResiduo = EstadoResiduo;
        ResiduoSeleccionado.IdCategoriaResiduo = CategoriaResiduoSeleccionada?.IdCategoriaResiduo ?? 0;

        await _residuoRepository.UpdateResiduoAsync(ResiduoSeleccionado);
        await Shell.Current.GoToAsync("..");
    }
}
