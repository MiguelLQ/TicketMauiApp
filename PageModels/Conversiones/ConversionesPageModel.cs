using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Conversiones;
public partial class ConversionesPageModel : ObservableObject
{
    public ObservableCollection<Convertidor> ListaConvertidor { get; } = new();
    private readonly IConvertidorRepository _convertidorRepository;
    private readonly IAlertaHelper _alertaHelper;
    private readonly SincronizacionFirebaseService? _sincronizador;
    [ObservableProperty]
    private bool _isBusy;
    public ConversionesPageModel(IConvertidorRepository convertidorRepository, IAlertaHelper alertaHelper, SincronizacionFirebaseService? sincronizador)
    {
        _convertidorRepository = convertidorRepository;
        _alertaHelper = alertaHelper;
        _sincronizador = sincronizador;
    }

    [RelayCommand]
    public async Task CargarConvertidoresAsync()
    {
        try
        {
            IsBusy = true;
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _sincronizador!.SincronizarConvertidoresDesdeFirebaseAsync();
            }

            ListaConvertidor.Clear();
            var convertidores = await _convertidorRepository.GetAllConvertidorAync();
            foreach (var item in convertidores)
            {
                ListaConvertidor.Add(item);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]

    public async Task CambiarEstadoConvertidor(string id)
    {
        await _convertidorRepository.ChangeEstadoConvertidorAsync(id);
        await _alertaHelper.ShowSuccessAsync("Se cambio de estado de manera exitosa");
        await CargarConvertidoresAsync();
    }
}
