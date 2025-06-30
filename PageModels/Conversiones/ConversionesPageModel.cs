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
    [ObservableProperty]
    private bool _isBusy;
    public ConversionesPageModel(IConvertidorRepository convertidorRepository, IAlertaHelper alertaHelper)
    {
        _convertidorRepository = convertidorRepository;
        _alertaHelper = alertaHelper;
    }

    [RelayCommand]
    public async Task CargarConvertidoresAsync()
    {
        try
        {
            IsBusy = true;
            await Task.Delay(1500);
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

    public async Task CambiarEstadoConvertidor(int id)
    {
        await _convertidorRepository.ChangeEstadoConvertidorAsync(id);
        await _alertaHelper.ShowSuccessAsync("Se cambio de estado de manera exitosa");
        await CargarConvertidoresAsync();
    }

    [RelayCommand]
    public async Task IrACrearResiduoAsync()
    {
        await Shell.Current.GoToAsync("AgregarConvertidorPage");
    }

}
