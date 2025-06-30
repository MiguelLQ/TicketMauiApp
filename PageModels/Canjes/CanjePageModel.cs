using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MauiFirebase.PageModels.Canjes;

public partial class CanjePageModel : ObservableObject
{
    private readonly ICanjeRepository _canjeRepository;
    private readonly IPremioRepository _premioRepository;
    private readonly IResidenteRepository _residenteRepository;
    public ObservableCollection<Canje> ListaCanjes { get; } = new();
    public ObservableCollection<Premio> ListaPremios { get; } = new();
    public ObservableCollection<Residente> ListaResidentes { get; } = new();

    [ObservableProperty] private bool _isBusy;

    public CanjePageModel(ICanjeRepository canjeRepository, IPremioRepository premioRepository, IResidenteRepository residenteRepository)
    {
        _canjeRepository = canjeRepository;
        _premioRepository = premioRepository;
        _residenteRepository = residenteRepository;
    }

    [RelayCommand]
    public async Task CargarPremioAsync()
    {
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
            await Task.Delay(800);
            ListaCanjes.Clear();
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

                if (premiosDict.TryGetValue(item.IdPremio, out var premio))
                {
                    item.NombrePremio = premio.NombrePremio;
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
    public async Task IrACrearResiduoAsync()
    {
        await Shell.Current.GoToAsync("CrearCanjePageModel");
    }
}