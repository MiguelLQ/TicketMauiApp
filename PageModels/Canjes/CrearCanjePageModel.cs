using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
namespace MauiFirebase.PageModels.Canjes;
public partial class CrearCanjePageModel : ObservableObject
{
    private readonly ICanjeRepository _canjeRepository;
    private readonly IPremioRepository _premioRepository;
    private readonly IResidenteRepository _residenteRepository;
    private readonly IAlertaHelper _alertaHelper;

    public ObservableCollection<Premio> ListaPremios { get; } = new();
    public ObservableCollection<Residente> ListaResidentes { get; } = new();
    public ObservableCollection<Canje> ListaCanje { get; } = new();

    [ObservableProperty] 
    private Premio? _premioSeleccionado;
    [ObservableProperty] 
    private string _dniResidente = string.Empty;
    [ObservableProperty] 
    private Residente? _residenteEncontrado;
    [ObservableProperty] 
    private bool _estadoCanje = true;

    public CrearCanjePageModel(
        ICanjeRepository canjeRepository,
        IPremioRepository premioRepository,
        IResidenteRepository residenteRepository,
        IAlertaHelper alertaHelper)
    {
        _canjeRepository = canjeRepository;
        _premioRepository = premioRepository;
        _residenteRepository = residenteRepository;
        _alertaHelper = alertaHelper;
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
            if (residentesDict.TryGetValue(item.IdResidente, out var residente))
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
            await _alertaHelper.ShowSuccessAsync($"Residente encontrado: {residente.NombreResidente}");
        }
        else
        {
            await _alertaHelper.ShowErrorAsync("Residente no encontrado.");
        }
    }

    [RelayCommand]
    public async Task CrearCanjeAsync()
    {
        var nuevoCanje = new Canje
        {
            FechaCanje = DateTime.Now,
            EstadoCanje = EstadoCanje,
            IdPremio = PremioSeleccionado?.IdPremio ?? 0,
            IdResidente = ResidenteEncontrado?.IdResidente ?? 0
        };

        await _canjeRepository.CreateCanjeAsync(nuevoCanje);
        await _alertaHelper.ShowSuccessAsync("Canje creado correctamente.");
        await Shell.Current.GoToAsync("..");
    }
}