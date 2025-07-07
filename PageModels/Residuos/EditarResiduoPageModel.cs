using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MauiFirebase.PageModels.Residuos;
[QueryProperty(nameof(IdResiduo), "id")]
public partial class EditarResiduoPageModel : ObservableValidator
{
    private readonly IResiduoRepository _residuoRepository;
    private readonly ICategoriaResiduoRepository _categoriaResiduoRepository;
    private readonly IAlertaHelper _alertaHelper;

    public ObservableCollection<CategoriaResiduo> ListaCategorias { get; } = new();

    [ObservableProperty]
    private int idResiduo;

    [ObservableProperty]
    private Residuo? _residuoSeleccionado;

    [ObservableProperty]
    [Required(ErrorMessage = "El nombre del residuo es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
    private string? _nombreResiduo;

    [ObservableProperty]
    private bool _estadoResiduo;

    [ObservableProperty]
    [Required(ErrorMessage = "El valor del residuo es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El valor debe ser mayor que 0.")]
    private int _valorResiduo;

    [ObservableProperty]
    [Required(ErrorMessage = "Debe seleccionar una categoría.")]
    private CategoriaResiduo? _categoriaResiduoSeleccionada;

    public EditarResiduoPageModel(IResiduoRepository residuoRepository, ICategoriaResiduoRepository categoriaResiduoRepository, IAlertaHelper alertaHelper)
    {
        _residuoRepository = residuoRepository;
        _categoriaResiduoRepository = categoriaResiduoRepository;
        _alertaHelper = alertaHelper;
    }

    public async Task InicializarAsync()
    {
        await CargarCategoriasAsync();

        ResiduoSeleccionado = await _residuoRepository.GetResiduoIdAsync(IdResiduo);

        if (ResiduoSeleccionado != null)
        {
            NombreResiduo = ResiduoSeleccionado.NombreResiduo;
            EstadoResiduo = ResiduoSeleccionado.EstadoResiduo;
            ValorResiduo = ResiduoSeleccionado.ValorResiduo;
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
        ValidateAllProperties();

        if (HasErrors)
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{errores}");
            return;
        }

        if (ResiduoSeleccionado == null)
        {
            return;
        }
        ResiduoSeleccionado.NombreResiduo = NombreResiduo!;
        ResiduoSeleccionado.EstadoResiduo = EstadoResiduo;
        ResiduoSeleccionado.ValorResiduo = ValorResiduo;
        ResiduoSeleccionado.IdCategoriaResiduo = CategoriaResiduoSeleccionada!.IdCategoriaResiduo;

        await _residuoRepository.UpdateResiduoAsync(ResiduoSeleccionado);
        await _alertaHelper.ShowSuccessAsync("Residuo actualizado correctamente.");
        await Shell.Current.GoToAsync("..");
    }

    /*==================================================================================
     *  VALIDACIONES DE PROPIEDADES PARA MOSTRAR ERRORES EN TIEMPO REAL
    ================================================================================= */
    partial void OnNombreResiduoChanged(string? value)
    {
        ValidateProperty(value, nameof(NombreResiduo));
        OnPropertyChanged(nameof(NombreResiduoError));
        OnPropertyChanged(nameof(HasNombreResiduoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnValorResiduoChanged(int value)
    {
        ValidateProperty(value, nameof(ValorResiduo));
        OnPropertyChanged(nameof(ValorResiduoError));
        OnPropertyChanged(nameof(HasValorResiduoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnCategoriaResiduoSeleccionadaChanged(CategoriaResiduo? value)
    {
        ValidateProperty(value, nameof(CategoriaResiduoSeleccionada));
        OnPropertyChanged(nameof(CategoriaResiduoError));
        OnPropertyChanged(nameof(HasCategoriaResiduoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }
    /* ===================================================================================
    * ERRORES PARA MOSTRAR EN TIEMPO REAL EN XAML
     ===================================================================================*/

    public string? NombreResiduoError => GetErrors(nameof(NombreResiduo)).FirstOrDefault()?.ErrorMessage;
    public string? ValorResiduoError => GetErrors(nameof(ValorResiduo)).FirstOrDefault()?.ErrorMessage;
    public string? CategoriaResiduoError => GetErrors(nameof(CategoriaResiduoSeleccionada)).FirstOrDefault()?.ErrorMessage;

    public bool HasNombreResiduoError => GetErrors(nameof(NombreResiduo)).Any();
    public bool HasValorResiduoError => GetErrors(nameof(ValorResiduo)).Any();
    public bool HasCategoriaResiduoError => GetErrors(nameof(CategoriaResiduoSeleccionada)).Any();
    public bool PuedeGuardar => !HasErrors && !string.IsNullOrWhiteSpace(NombreResiduo) && ValorResiduo > 0 && CategoriaResiduoSeleccionada != null;
}