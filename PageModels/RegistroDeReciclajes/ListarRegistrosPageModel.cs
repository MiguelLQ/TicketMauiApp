using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.RegistroDeReciclajes;
public partial class ListarRegistrosPageModel : ObservableObject
{
    public ObservableCollection<RegistroDeReciclaje> ListaRegistrosResiduo { get; } = new();
    public ObservableCollection<Residente> ListaResidentes { get; } = new();
    public ObservableCollection<Residuo> ListaResiduos { get; } = new();
    private readonly IRegistroDeReciclajeRepository _registroRepository;
    private readonly IResidenteRepository _residenteRepository;
    private readonly IResiduoRepository _residuoRepository;

    public ListarRegistrosPageModel(IRegistroDeReciclajeRepository registroRepository, IResidenteRepository residenteRepository, IResiduoRepository residuoRepository)
    {
        _registroRepository = registroRepository;
        _residenteRepository = residenteRepository;
        _residuoRepository = residuoRepository;

    }

    [RelayCommand]
    public async Task CargarResiduoAsync()
    {
        ListaResiduos.Clear();
        var residuos = await _residuoRepository.GetAllResiduoAync();
        foreach (var item in residuos)
        {
            ListaResiduos.Add(item);
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
    public async Task CargarRegistroResiduoAsync()
    {
        ListaRegistrosResiduo.Clear();
        var registros = await _registroRepository.ObtenerTodosAsync();
        var residentes = await _residenteRepository.GetAllResidentesAsync();
        var residuos = await _residuoRepository.GetAllResiduoAync();
        var residentesDict = residentes.ToDictionary(r => r.IdResidente);
        var residuosDict = residuos.ToDictionary(r => r.IdResiduo);
        foreach (var item in registros)
        {
            if (residentesDict.TryGetValue(item.IdResidente, out var residente))
            {
                item.NombreResidente = residente.NombreResidente;
            }
            if (residuosDict.TryGetValue(item.IdResiduo, out var residuo))
            {
                item.NombreResiduo = residuo.NombreResiduo;
            }
            ListaRegistrosResiduo.Add(item);
        }
    }
    [RelayCommand]
    public async Task IrACrearResiduoAsync()
    {
        await Shell.Current.GoToAsync("AgregarRegistroPageModel");
    }

    [RelayCommand]

    public async Task BuscarResidente()
    {
        await Shell.Current.GoToAsync("BuscarResidentePageModel");
    }
}
