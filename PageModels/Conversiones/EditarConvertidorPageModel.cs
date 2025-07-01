using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace MauiFirebase.PageModels.Conversiones;
[QueryProperty(nameof(IdConvertidor), "id")]

public partial class EditarConvertidorPageModel : ObservableValidator
{
    private readonly IConvertidorRepository _convertidorRepository;
    private readonly IAlertaHelper _alertaHelper;
    public ObservableCollection<Convertidor> ListaConversiones { get; } = new();
    [ObservableProperty]
    private int _idConvertidor;
    [ObservableProperty]
    [Required(ErrorMessage = "El valor mínimo es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El valor mínimo debe ser un número positivo.")]
    private int? _valorMin;

    [ObservableProperty]
    [Required(ErrorMessage = "El valor máximo es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El valor máximo debe ser un número positivo.")]
    private int? _valorMax;

    [ObservableProperty]
    [Required(ErrorMessage = "El número de ticket es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe tener al menos 1 ticket.")]
    private int? _numeroTicket;

    [ObservableProperty]
    private bool _estadoConvertidor = true;

    public EditarConvertidorPageModel(IConvertidorRepository convertidorRepository, IAlertaHelper alertaHelper)
    {
        _alertaHelper = alertaHelper;
        _convertidorRepository = convertidorRepository;
    }

    public async Task InicializarAsync()
    {
        await CargarConvertidoresAsync();
        var convertidor = await _convertidorRepository.GetConvertidorIdAsync(IdConvertidor);
        if (convertidor != null)
        {
            ValorMin = convertidor.ValorMin;
            ValorMax = convertidor.ValorMax;
            NumeroTicket = convertidor.NumeroTicket;
            EstadoConvertidor = convertidor.EstadoConvertidor;
        }
    }
    [RelayCommand]
    public async Task CargarConvertidoresAsync()
    {
        ListaConversiones.Clear();
        var conversiones = await _convertidorRepository.GetAllConvertidorAync();
        foreach (var item in conversiones)
        {
            ListaConversiones.Add(item);
        }
    }

    [RelayCommand]
    public async Task GuardarCambiosAsync()
    {
        var convertidorActualizado = new Convertidor
        {
            IdConvertidor = IdConvertidor,
            ValorMin = ValorMin!.Value,
            ValorMax = ValorMax!.Value,
            NumeroTicket = NumeroTicket!.Value,
            EstadoConvertidor = EstadoConvertidor
        };

        await _convertidorRepository.UpdateConvertidorAsync(convertidorActualizado);
        await _alertaHelper.ShowSuccessAsync("Convertidor actualizado correctamente.");
        await Shell.Current.GoToAsync("..");
    }
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


    public string? ValorMinError => GetErrors(nameof(ValorMin)).FirstOrDefault()?.ErrorMessage;
    public string? ValorMaxError => GetErrors(nameof(ValorMax)).FirstOrDefault()?.ErrorMessage;
    public string? NumeroTicketError => GetErrors(nameof(NumeroTicket)).FirstOrDefault()?.ErrorMessage;
    public bool HasValorMinError => GetErrors(nameof(ValorMin)).Any();
    public bool HasValorMaxMayorQueValorMinError => !string.IsNullOrEmpty(ValorMaxMayorQueValorMinError);
    public bool HasValorMaxError => GetErrors(nameof(ValorMax)).Any();
    public bool HasNumeroTicketError => GetErrors(nameof(NumeroTicket)).Any();
    public bool PuedeGuardar => !HasErrors && ValorMin > 0 && ValorMax > ValorMin &&  NumeroTicket > 0;
}
