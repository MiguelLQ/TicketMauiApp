using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Rutas;

public partial class RutaPageModel : ObservableObject
{
    public ObservableCollection<Ruta> ListaRutas { get; } = new();

    private readonly IRutaRepository _rutaRepository;
    private readonly IAlertaHelper _alertaHelper;

    [ObservableProperty]
    private bool isBusy;

    public RutaPageModel(IRutaRepository rutaRepository, IAlertaHelper alertaHelper)
    {
        _rutaRepository = rutaRepository;
        _alertaHelper = alertaHelper;
    }

    [RelayCommand]
    public async Task CargarRutasAsync()
    {
        try
        {
            IsBusy = true;
            ListaRutas.Clear();
            var rutas = await _rutaRepository.GetAllRutaAsync();
            foreach (var ruta in rutas)
            {
                ListaRutas.Add(ruta);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CambiarEstadoRutaAsync(string id)
    {
        await _rutaRepository.ChangeEstadoRutaAsync(id);
        await _alertaHelper.ShowSuccessAsync("Se cambió el estado de manera exitosa");
        await CargarRutasAsync();
    }

    [RelayCommand]
    public async Task IrACrearRutaAsync()
    {
        await Shell.Current.GoToAsync("AgregarRutaPage");
    }
}
