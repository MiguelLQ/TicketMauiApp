using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Helpers.Interface;
using System.ComponentModel.DataAnnotations;

namespace MauiFirebase.PageModels.Residentes;

public partial class ResidenteFormPageModel : ObservableValidator
{
    private readonly IResidenteRepository _residenteRepository;
    private readonly IAlertaHelper _alertaHelper;

    public ResidenteFormPageModel(IResidenteRepository residenteRepository, IAlertaHelper alertaHelper)
    {
        _residenteRepository = residenteRepository;
        _alertaHelper = alertaHelper;
    }

    // ====================== PROPIEDADES ======================

    [ObservableProperty]
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 20 caracteres.")]
    private string nombreResidente = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "El apellido debe tener entre 3 y 20 caracteres.")]
    private string apellidoResidente = string.Empty;

    [ObservableProperty]
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos.")]
    private string dniResidente = string.Empty;

    [ObservableProperty]
    private string correoResidente = string.Empty;

    [ObservableProperty]
    private string direccionResidente = string.Empty;

    [ObservableProperty]
    private bool estadoResidente = true;

    public string? DniDuplicadoError { get; private set; }
    public bool HasDniDuplicadoError => !string.IsNullOrWhiteSpace(DniDuplicadoError);

    // ====================== COMANDOS ======================

    [RelayCommand]
    public async Task CrearResidenteAsync()
    {
        ValidateAllProperties();
        if (HasErrors || HasDniDuplicadoError)
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            if (HasDniDuplicadoError)
                errores += $"\n{DniDuplicadoError}";
            await _alertaHelper.ShowErrorAsync($"Errores:\n{errores}");
            return;
        }
        var nuevo = new Residente
        {
            NombreResidente = NombreResidente,
            ApellidoResidente = ApellidoResidente,
            DniResidente = DniResidente,
            CorreoResidente = CorreoResidente,
            DireccionResidente = DireccionResidente,
            EstadoResidente = EstadoResidente,
            FechaRegistroResidente = DateTime.Now
        };

        
        await _residenteRepository.CreateResidenteAsync(nuevo);
        await _alertaHelper.ShowSuccessAsync("Ciudadano Registrado Correctamente.");
        
        await Shell.Current.GoToAsync("..");
        
    }
    public void LimpiarFormulario()
    {
        NombreResidente = string.Empty;
        ApellidoResidente = string.Empty;
        DniResidente = string.Empty;
        CorreoResidente = string.Empty;
        DireccionResidente = string.Empty;
        EstadoResidente = true;
        DniDuplicadoError = null;
        ClearErrors();
    }
    // ====================== VALIDACIÓN EN TIEMPO REAL ======================

    partial void OnNombreResidenteChanged(string value)
    {
        ValidateProperty(value, nameof(NombreResidente));
        OnPropertyChanged(nameof(NombreResidenteError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnApellidoResidenteChanged(string value)
    {
        ValidateProperty(value, nameof(ApellidoResidente));
        OnPropertyChanged(nameof(ApellidoResidenteError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnDniResidenteChanged(string value)
    {
        ValidateProperty(value, nameof(DniResidente));
        _ = VerificarDniDuplicadoAsync(value);
        OnPropertyChanged(nameof(DniResidenteError));
        OnPropertyChanged(nameof(DniDuplicadoError));
        OnPropertyChanged(nameof(HasDniDuplicadoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnCorreoResidenteChanged(string value)
    {
        ValidateProperty(value, nameof(CorreoResidente));
        OnPropertyChanged(nameof(CorreoResidenteError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnDireccionResidenteChanged(string value)
    {
        ValidateProperty(value, nameof(DireccionResidente));
        OnPropertyChanged(nameof(DireccionResidenteError));
        OnPropertyChanged(nameof(HasDireccionResidenteError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    private async Task VerificarDniDuplicadoAsync(string dni)
    {
        DniDuplicadoError = null;

        if (!string.IsNullOrWhiteSpace(dni) && dni.Length == 8)
        {
            var existente = await _residenteRepository.GetResidenteByDniAsync(dni);
            if (existente != null)
            {
                DniDuplicadoError = "Ya existe un residente con este DNI.";
            }
        }

        OnPropertyChanged(nameof(DniDuplicadoError));
        OnPropertyChanged(nameof(HasDniDuplicadoError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    // =======================================================
                    // ERRORES PARA XAML
    // ========================================================

    public string? NombreResidenteError => GetErrors(nameof(NombreResidente)).FirstOrDefault()?.ErrorMessage;
    public string? ApellidoResidenteError => GetErrors(nameof(ApellidoResidente)).FirstOrDefault()?.ErrorMessage;
    public string? DniResidenteError => GetErrors(nameof(DniResidente)).FirstOrDefault()?.ErrorMessage;
    public string? CorreoResidenteError => GetErrors(nameof(CorreoResidente)).FirstOrDefault()?.ErrorMessage;
    public string? DireccionResidenteError => GetErrors(nameof(DireccionResidente)).FirstOrDefault()?.ErrorMessage;
    public bool HasDireccionResidenteError => !string.IsNullOrWhiteSpace(DireccionResidenteError);

    public bool PuedeGuardar => !HasErrors && !HasDniDuplicadoError && !string.IsNullOrWhiteSpace(NombreResidente) && !string.IsNullOrWhiteSpace(ApellidoResidente) && !string.IsNullOrWhiteSpace(DniResidente);
}
