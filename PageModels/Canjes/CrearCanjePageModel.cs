using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
namespace MauiFirebase.PageModels.Canjes;
public partial class CrearCanjePageModel : ObservableValidator
{
    private readonly ICanjeRepository _canjeRepository;
    private readonly IPremioRepository _premioRepository;
    private readonly IResidenteRepository _residenteRepository;
    private readonly IAlertaHelper _alertaHelper;
    private readonly SincronizacionFirebaseService _sincronizador;
    public ObservableCollection<Premio> ListaPremios { get; } = new();
    public ObservableCollection<Residente> ListaResidentes { get; } = new();
    public ObservableCollection<Canje> ListaCanje { get; } = new();
    public ObservableCollection<Premio> PremiosDisponibles { get; } = new();

    [ObservableProperty]
    private Premio? _premioSeleccionado;// idpremio 
    [ObservableProperty]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos.")]
    private string _dniResidente = string.Empty;
    [ObservableProperty]
    private Residente? _residenteEncontrado;//idresidente
    [ObservableProperty]
    private DateTime _fechaDeCanjeo = DateTime.Now;
    [ObservableProperty]
    private bool _estadoCanje = true;//defecto en true
    [ObservableProperty]
    private bool _noTienePremiosDisponibles;

    public bool PuedeGuardar
    {
        get
        {
            if (ResidenteEncontrado == null)
            {
                return false;
            }
            if (PremioSeleccionado == null)
            {
                return false;
            }
            if (ResidenteEncontrado.TicketsTotalesGanados < PremioSeleccionado.PuntosRequeridos)
            {
                return false;
            }

            return true;
        }
    }

    partial void OnPremioSeleccionadoChanged(Premio? value)
    {
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    partial void OnResidenteEncontradoChanged(Residente? value)
    {
        OnPropertyChanged(nameof(PuedeGuardar));
    }

    public CrearCanjePageModel(
        ICanjeRepository canjeRepository,
        IPremioRepository premioRepository,
        IResidenteRepository residenteRepository,
        IAlertaHelper alertaHelper,
        SincronizacionFirebaseService sincronizador)
    {
        _canjeRepository = canjeRepository;
        _premioRepository = premioRepository;
        _residenteRepository = residenteRepository;
        _alertaHelper = alertaHelper;
        _sincronizador = sincronizador;


    }

    [RelayCommand]
    public async Task CargarPremiosAsync()
    {

        ListaPremios.Clear();
        var premios = await _premioRepository.GetAllPremiosAsync();
        foreach (var premio in premios)
            ListaPremios.Add(premio);
    }

    [RelayCommand]
    public async Task CargarResiduosAsync()
    {
        ListaResidentes.Clear();
        var residente = await _residenteRepository.GetAllResidentesAsync();
        foreach (var item in residente)
        {
            ListaResidentes.Add(item);
        }
    }

    [RelayCommand]
    public async Task CargarCanjeAsync()
    {
        ListaCanje.Clear();
        var canjes = await _canjeRepository.GetAllCanjeAync();
        var premios = await _premioRepository.GetAllPremiosAsync();
        var residentes = await _residenteRepository.GetAllResidentesAsync();


        var residentesDict = residentes.ToDictionary(r => r.IdResidente);
        var premiosDict = premios.ToDictionary(r => r.IdPremio);

        foreach (var item in canjes)
        {
            if (residentesDict.TryGetValue(item.IdResidente!, out var residente))
            {
                item.NombreResidente = residente.NombreResidente;
            }
            if (premiosDict.TryGetValue(item.IdCanje, out var canje))
            {
                item.NombrePremio = canje.NombrePremio;
            }
            ListaCanje.Add(item);
        }
    }


    [RelayCommand]
    public async Task BuscarResidenteAsync()
    {

        if (string.IsNullOrWhiteSpace(DniResidente))
        {
            await _alertaHelper.ShowErrorAsync("Ingrese un DNI válido.");
            return;
        }

        var residente = await _residenteRepository.ObtenerPorDniAsync(DniResidente);
        if (residente != null)
        {
            ResidenteEncontrado = residente;
            await _alertaHelper.ShowSuccessAsync($"Residente encontrado");
            await ActualizarPremiosDisponiblesAsync();
        }
        else
        {
            await _alertaHelper.ShowErrorAsync("Residente no encontrado.");
            ResidenteEncontrado = null;
            PremiosDisponibles.Clear();
        }
    }

    [RelayCommand]
    public async Task CrearCanjeAsync()
    {
        if (ResidenteEncontrado == null)
        {
            await _alertaHelper.ShowErrorAsync("Debe buscar un residente primero.");
            return;
        }
        if (PremioSeleccionado == null)
        {
            await _alertaHelper.ShowErrorAsync("Debe seleccionar un premio.");
            return;
        }
        if (ResidenteEncontrado.TicketsTotalesGanados < PremioSeleccionado.PuntosRequeridos)
        {
            await _alertaHelper.ShowErrorAsync($"El residente no tiene suficientes puntos. Tiene {ResidenteEncontrado.TicketsTotalesGanados} y el premio cuesta {PremioSeleccionado.PuntosRequeridos}.");
            return;
        }

        ResidenteEncontrado.TicketsTotalesGanados -= PremioSeleccionado.PuntosRequeridos;
        await _residenteRepository.UpdateResidenteAsync(ResidenteEncontrado);
        await _sincronizador.SincronizarResidentesAsync();

        var nuevoCanje = new Canje
        {
            FechaCanje = FechaDeCanjeo,
            EstadoCanje = EstadoCanje,
            IdPremio = PremioSeleccionado.IdPremio,
            IdResidente = ResidenteEncontrado.IdResidente,
            Sincronizado = false
        };
        await _canjeRepository.CreateCanjeAsync(nuevoCanje);

        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            try
            {
                await _sincronizador.SincronizarCanjeAsync();
            }
            catch
            {
                await _alertaHelper.ShowWarningAsync("Guardado localmente. Se sincronizará cuando haya internet.");
            }
        }

        await _alertaHelper.ShowSuccessAsync("Canje creado correctamente.");
        await Shell.Current.GoToAsync("..");
    }

    private async Task ActualizarPremiosDisponiblesAsync()
    {
        PremiosDisponibles.Clear();

        if (ResidenteEncontrado == null)
        {
            return;
        }

        var todosLosPremios = await _premioRepository.GetAllPremiosAsync();

        var premiosFiltrados = todosLosPremios
            .Where(p => ResidenteEncontrado.TicketsTotalesGanados >= p.PuntosRequeridos)
            .ToList();

        NoTienePremiosDisponibles = !premiosFiltrados.Any();

        foreach (var premio in premiosFiltrados)
        {
            PremiosDisponibles.Add(premio);
        }
    }
    partial void OnDniResidenteChanged(string value)
    {
        var cleaned = new string(value.Where(char.IsDigit).ToArray());
        if (cleaned.Length > 8)
        {
            cleaned = cleaned.Substring(0, 8);
        }

        if (cleaned != value)
        {
            DniResidente = cleaned;
            return;
        }

        if (cleaned.Length < 8)
        {
            ResidenteEncontrado = null;
            PremiosDisponibles.Clear();
            NoTienePremiosDisponibles = false;
            return;
        }
        if (cleaned.Length == 8)
        {
            _ = BuscarResidenteAsync();
        }
    }
}