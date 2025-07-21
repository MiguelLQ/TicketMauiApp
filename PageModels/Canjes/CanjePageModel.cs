using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Canjes;

public partial class CanjePageModel : ObservableObject
{
    private readonly ICanjeRepository _canjeRepository;
    private readonly IPremioRepository _premioRepository;
    private readonly IResidenteRepository _residenteRepository;
    private readonly SincronizacionFirebaseService _sincronizar;
    public ObservableCollection<Canje> ListaCanjes { get; } = new();
    public ObservableCollection<Premio> ListaPremios { get; } = new();
    public ObservableCollection<Residente> ListaResidentes { get; } = new();

    [ObservableProperty] private bool _isBusy;

    public CanjePageModel(ICanjeRepository canjeRepository, IPremioRepository premioRepository, IResidenteRepository residenteRepository, SincronizacionFirebaseService sincronizar)
    {
        _canjeRepository = canjeRepository;
        _premioRepository = premioRepository;
        _residenteRepository = residenteRepository;
        _sincronizar = sincronizar;
    }

    [RelayCommand]
    public async Task CargarPremioAsync()
    {
        try
        {
            IsBusy = true;
            if (_sincronizar != null && Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _sincronizar.SincronizarCanjesDesdeFirebaseAsync(); 
                await _sincronizar.SincronizarCanjeAsync();

            }

            await CargarResidentesAsync();
            await CargarCanjeAsync();
        }
        finally
        {
            IsBusy = false;
        }
        ListaPremios.Clear();
        var residuos = await _premioRepository.GetAllPremiosAsync();
        foreach (var item in residuos)
        {
            ListaPremios.Add(item);
        }
    }

    [RelayCommand]
    public async Task CargarResidentesAsync()
    {
        ListaResidentes.Clear();
        var residentes = await _residenteRepository.GetAllResidentesAsync();
        foreach (var item in residentes)
        {
            ListaResidentes.Add(item);
        }
    }

    [RelayCommand]
    public async Task CargarCanjeAsync()
    {
        try
        {
            IsBusy = true;
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _sincronizar!.SincronizarCanjesDesdeFirebaseAsync();
            }

            ListaCanjes.Clear();
            var canjes = await _canjeRepository.GetAllCanjeAync();
            var premios = await _premioRepository.GetAllPremiosAsync();
            var residentes = await _residenteRepository.GetAllResidentesAsync();
            var residentesDict = residentes.ToDictionary(r => r.IdResidente);
            var premiosDict = premios.ToDictionary(r => r.IdPremio);

            // Tomar los 5 últimos registros por fecha (descendente)
            var ultimosCanjes = canjes
                .OrderByDescending(c => c.FechaCanje) // Asegúrate que FechaCanje existe y es DateTime
                .Take(5)
                .ToList();

            foreach (var item in ultimosCanjes)
            {
                if (residentesDict.TryGetValue(item.IdResidente!, out var residente))
                {
                    item.NombreResidente = residente.NombreResidente;
                    item.ApellidoResidente = residente.ApellidoResidente;
                }

                if (premiosDict.TryGetValue(item.IdPremio!, out var premio))
                {
                    item.NombrePremio = premio.NombrePremio;
                    item.DescripcionPremio = premio.NombrePremio;
                }
                ListaCanjes.Add(item);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task IrACrearCanjeAsync()
    {
        await Shell.Current.GoToAsync("AgregarCanjePageModel");
    }
}