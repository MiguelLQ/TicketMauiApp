using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Conversiones;
[QueryProperty(nameof(IdConvertidor), "id")]

public partial class EditarConvertidorPageModel : ObservableObject
{
    private readonly IConvertidorRepository _convertidorRepository;
    private readonly IAlertaHelper _alertaHelper;
    public ObservableCollection<Convertidor> ListaConversiones { get; } = new();
    [ObservableProperty]
    private int _idConvertidor;
    [ObservableProperty]
    private int _valorMin;
    [ObservableProperty]
    private int _valorMax;
    [ObservableProperty]
    private int _numeroTicket;
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
    public async  Task CargarConvertidoresAsync()
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
            ValorMin = ValorMin,
            ValorMax = ValorMax,
            NumeroTicket = NumeroTicket,
            EstadoConvertidor = EstadoConvertidor
        };

        await _convertidorRepository.UpdateConvertidorAsync(convertidorActualizado);
        await _alertaHelper.ShowSuccessAsync("Convertidor actualizado correctamente.");
        await Shell.Current.GoToAsync("..");
    }

}
