using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
namespace MauiFirebase.PageModels.Residentes;

public partial class ResidenteListPageModel : ObservableValidator
{
    private readonly IResidenteRepository _residenteRepository;
    private readonly IAlertaHelper _alertaHelper;
    private readonly SincronizacionFirebaseService? _sincronizador;
    public ObservableCollection<Residente> ListaResidentes { get; } = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string filtro = "Todos";

    [ObservableProperty]
    [CustomValidation(typeof(ResidenteListPageModel), nameof(ValidarDni))]
    private string textoBusqueda = string.Empty;

    private List<Residente> _respaldoResidentes = new();

    public ResidenteListPageModel(IResidenteRepository residenteRepository, IAlertaHelper alertaHelper, SincronizacionFirebaseService? sincronizador)
    {
        _residenteRepository = residenteRepository;
        _alertaHelper = alertaHelper;
        _sincronizador = sincronizador;
    }

    // ===============================
    // Validación personalizada de DNI
    // ===============================
    public static ValidationResult? ValidarDni(string dni, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(dni))
        {
            return ValidationResult.Success;
        }
        if (dni.Length != 8 || !dni.All(char.IsDigit))
        {
            return new ValidationResult("El DNI debe tener exactamente 8 dígitos numéricos.");
        }
        return ValidationResult.Success;
    }


    [RelayCommand]
    public async Task CargarResidentesAsync()
    {
        try
        {
            IsBusy = true;

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _sincronizador!.SincronizarResidentesDesdeFirebaseAsync();
            }
            ListaResidentes.Clear();
            var residentes = await _residenteRepository.GetAllResidentesAsync();
            _respaldoResidentes = residentes.ToList();
            AplicarFiltros();
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
    public void AplicarFiltros()
    {

        if (HasTextoBusquedaError)
        {
            ListaResidentes.Clear();
            return;
        }

        var filtrados = string.IsNullOrWhiteSpace(TextoBusqueda)
            ? _respaldoResidentes
            : _respaldoResidentes.Where(r => r.DniResidente != null && r.DniResidente.Contains(TextoBusqueda, StringComparison.OrdinalIgnoreCase));

        ListaResidentes.Clear();
        foreach (var r in filtrados)
        {
            ListaResidentes.Add(r);
        }
        if (!filtrados.Any() && !string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            _ = _alertaHelper.ShowErrorAsync("No se encontró ningún residente con ese número de DNI");
        }
    }


    [RelayCommand]
    public async Task IrACrearResidenteAsync()
    {
        await Shell.Current.GoToAsync("ResidenteFormPage");
    }

    // ===================================
    // Manejo de cambios en tiempo real
    // ==================================
    partial void OnTextoBusquedaChanged(string value)
    {
        ValidateProperty(value, nameof(TextoBusqueda));
        OnPropertyChanged(nameof(TextoBusquedaError));
        OnPropertyChanged(nameof(HasTextoBusquedaError));

        AplicarFiltros();
    }


    // ==========================================
    // Propiedades para errores en XAML
    // ===========================================
    public string? TextoBusquedaError => GetErrors(nameof(TextoBusqueda)).FirstOrDefault()?.ErrorMessage;
    public bool HasTextoBusquedaError => GetErrors(nameof(TextoBusqueda)).Any();
}
