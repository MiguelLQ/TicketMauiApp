using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using Microsoft.Maui.Storage;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Premios;

public partial class CrearPremioPageModel : ObservableValidator
{
    [ObservableProperty]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Debe tener entre 3 y 50 caracteres.")]
    private string? nombrePremio;

    [ObservableProperty]
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MinLength(5, ErrorMessage = "Debe tener al menos 5 caracteres.")]
    private string? descripcionPremio;

    [ObservableProperty]
    [Required(ErrorMessage = "Los puntos es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe ingresar puntos mayores a 0.")]
    private int? puntosRequeridos;

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


    [RelayCommand(CanExecute = nameof(PuedeGuardar))]
    public async Task AddPremioAsync()
    {
        ValidateAllProperties();

        if (HasErrors || string.IsNullOrEmpty(FotoPremio))
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            if (string.IsNullOrEmpty(FotoPremio))
                errores += "\nDebe seleccionar una imagen.";

            await _alertaHelper.ShowErrorAsync($"Errores:\n{errores}");
            return;
        }

        var nuevo = new Premio
        {
            NombrePremio = NombrePremio!,
            DescripcionPremio = DescripcionPremio!,
            PuntosRequeridos = PuntosRequeridos!.Value,
            EstadoPremio = EstadoPremio,
            FotoPremio = FotoPremio
        };

        await _premioRepository.CreatePremioAsync(nuevo);
        await _alertaHelper.ShowSuccessAsync("Premio creado correctamente.");
        await Shell.Current.GoToAsync("..");
    }
    private void LimpiarFormulario()
    {
        IdPremio = 0;
        NombrePremio = string.Empty;
        DescripcionPremio = string.Empty;
        PuntosRequeridos = 0;
        EstadoPremio = false;
        FotoPremio = string.Empty;
    }

    /*==================================================================================
     *  VALIDACIONES DE PROPIEDADES PARA MOSTRAR ERRORES EN TIEMPO REAL
    ================================================================================= */

    partial void OnNombrePremioChanged(string? value)
    {
        ValidateProperty(value, nameof(NombrePremio));
        OnPropertyChanged(nameof(NombrePremioError));
        OnPropertyChanged(nameof(HasNombrePremioError));
        AddPremioCommand.NotifyCanExecuteChanged();
    }

    partial void OnDescripcionPremioChanged(string? value)
    {
        ValidateProperty(value, nameof(DescripcionPremio));
        OnPropertyChanged(nameof(DescripcionPremioError));
        OnPropertyChanged(nameof(HasDescripcionPremioError));
        AddPremioCommand.NotifyCanExecuteChanged();
    }

    partial void OnPuntosRequeridosChanged(int? value)
    {
        ValidateProperty(value, nameof(PuntosRequeridos));
        OnPropertyChanged(nameof(PuntosRequeridosError));
        OnPropertyChanged(nameof(HasPuntosRequeridosError));
        AddPremioCommand.NotifyCanExecuteChanged();
    }

    partial void OnFotoPremioChanged(string? value)
    {
        OnPropertyChanged(nameof(HasFotoError));
        AddPremioCommand.NotifyCanExecuteChanged();
    }

    /* ===============================================================================
    * ERRORES PARA MOSTRAR EN TIEMPO REAL EN XAML
    ===================================================================================*/
    public string? NombrePremioError => GetErrors(nameof(NombrePremio)).FirstOrDefault()?.ErrorMessage;
    public string? DescripcionPremioError => GetErrors(nameof(DescripcionPremio)).FirstOrDefault()?.ErrorMessage;
    public string? PuntosRequeridosError => GetErrors(nameof(PuntosRequeridos)).FirstOrDefault()?.ErrorMessage;

    public bool HasNombrePremioError => GetErrors(nameof(NombrePremio)).Any();
    public bool HasDescripcionPremioError => GetErrors(nameof(DescripcionPremio)).Any();
    public bool HasPuntosRequeridosError => GetErrors(nameof(PuntosRequeridos)).Any();
    public bool HasFotoError => string.IsNullOrEmpty(FotoPremio);

    public bool PuedeGuardar => !HasErrors && !string.IsNullOrWhiteSpace(NombrePremio) && !string.IsNullOrWhiteSpace(DescripcionPremio) && PuntosRequeridos > 0 && !string.IsNullOrEmpty(FotoPremio);

}
