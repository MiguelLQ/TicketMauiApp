using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MauiFirebase.PageModels.Conversiones;
public partial class CrearConvertidorPageModel : ObservableValidator
{
    [ObservableProperty]
    [Required(ErrorMessage = "El valor mínimo es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El valor mínimo debe ser un número positivo.")]
    private int? _valorMin;

    [ObservableProperty]
    [Required(ErrorMessage = "El valor máximo es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El valor máximo debe ser mayor que 0.")]
    private int? _valorMax;

    [ObservableProperty]
    [Required(ErrorMessage = "El número de ticket es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe tener al menos 1 ticket.")]
    private int? _numeroTicket;

    [ObservableProperty]
    private bool _estadoConvertidor = true;
    public ObservableCollection<Convertidor> ListaConversiones { get; } = new();
    private readonly IConvertidorRepository _convertidorRepository;
    private readonly IAlertaHelper _alertaHelper;
    public CrearConvertidorPageModel(IConvertidorRepository convertidorRepository, IAlertaHelper alertaHelper)
    {
        _convertidorRepository = convertidorRepository;
        _alertaHelper = alertaHelper;
    }
    [RelayCommand]

    public async Task CargarConvertidorAsync()
    {
        ListaConversiones.Clear();
        var conversiones = await _convertidorRepository.GetAllConvertidorAync();
        foreach (var item in conversiones)
        {
            ListaConversiones.Add(item);
        }
    }

    [RelayCommand]
    public async Task CrearConvertidorAsync()
    {
        ValidateAllProperties();

        if (HasErrors)
        {
            var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
            await _alertaHelper.ShowErrorAsync($"Errores de validación:\n{errores}");
            return;
        }

        if (ValorMax <= ValorMin)
        {
            await _alertaHelper.ShowErrorAsync("El valor máximo debe ser mayor que el valor mínimo.");
            return;
        }

        var nuevo = new Convertidor
        {
            ValorMin = ValorMin!.Value,
            ValorMax = ValorMax!.Value,
            NumeroTicket = NumeroTicket!.Value,
            EstadoConvertidor = EstadoConvertidor
        };

        await _convertidorRepository.CreateConvertidorAsync(nuevo);
        await _alertaHelper.ShowSuccessAsync("Convertidor creado correctamente.");
        await Shell.Current.GoToAsync("..");
    }
    /*==================================================================================
     *  VALIDACIONES DE PROPIEDADES PARA MOSTRAR ERRORES EN TIEMPO REAL
     ================================================================================= */

    public string? ValorMaxMayorQueValorMinError
    {
        get
        {
            if (ValorMin.HasValue && ValorMax.HasValue && ValorMax <= ValorMin)
            {
                return "El valor máximo debe ser mayor que el valor mínimo.";
            }
            return null;
        }
    }
    partial void OnValorMinChanged(int? value)
    {
        ValidateProperty(value, nameof(ValorMin));
        OnPropertyChanged(nameof(ValorMinError));
        OnPropertyChanged(nameof(HasValorMinError));
        OnPropertyChanged(nameof(ValorMaxMayorQueValorMinError));
        OnPropertyChanged(nameof(HasValorMaxMayorQueValorMinError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnValorMaxChanged(int? value)
    {
        ValidateProperty(value, nameof(ValorMax));
        OnPropertyChanged(nameof(ValorMaxError));
        OnPropertyChanged(nameof(HasValorMaxError));
        OnPropertyChanged(nameof(ValorMaxMayorQueValorMinError));
        OnPropertyChanged(nameof(HasValorMaxMayorQueValorMinError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnNumeroTicketChanged(int? value)
    {
        ValidateProperty(value, nameof(NumeroTicket));
        OnPropertyChanged(nameof(NumeroTicketError));
        OnPropertyChanged(nameof(HasNumeroTicketError));
        OnPropertyChanged(nameof(PuedeGuardar));
    }


    /* ===============================================================================
     * ERRORES PARA MOSTRAR EN TIEMPO REAL EN XAML
    ===================================================================================*/
    public string? ValorMinError => GetErrors(nameof(ValorMin)).FirstOrDefault()?.ErrorMessage;
    public string? ValorMaxError => GetErrors(nameof(ValorMax)).FirstOrDefault()?.ErrorMessage;
    public string? NumeroTicketError => GetErrors(nameof(NumeroTicket)).FirstOrDefault()?.ErrorMessage;



    public bool HasValorMaxMayorQueValorMinError =>
        !string.IsNullOrEmpty(ValorMaxMayorQueValorMinError);
    public bool PuedeGuardar => !HasErrors && ValorMin.HasValue && ValorMax.HasValue && NumeroTicket.HasValue && ValorMax > ValorMin;
    public bool HasValorMinError => GetErrors(nameof(ValorMin)).Any();
    public bool HasValorMaxError => GetErrors(nameof(ValorMax)).Any();
    public bool HasNumeroTicketError => GetErrors(nameof(NumeroTicket)).Any();
}
