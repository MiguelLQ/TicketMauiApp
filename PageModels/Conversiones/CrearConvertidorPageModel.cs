using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Conversiones;
public partial class CrearConvertidorPageModel: ObservableObject
{
    [ObservableProperty]
    private int _valorMin;
    [ObservableProperty]
    private int _valorMax;
    [ObservableProperty]
    private int _numeroTicket;
    [ObservableProperty]
    private bool _estadoConvertidor = true;
    public ObservableCollection<Convertidor> ListaConversiones { get; } = new();
    private readonly IConvertidorRepository _convertidorRepository;
    private readonly IAlertaHelper _alertaHelper;
    public CrearConvertidorPageModel(IConvertidorRepository convertidorRepository, IAlertaHelper alertaHelper)
    {
        _convertidorRepository = convertidorRepository;
        _alertaHelper = alertaHelper;
    }
    [RelayCommand]

    public async Task CargarConvertidorAsync()
    {
        ListaConversiones.Clear();
        var conversiones = await _convertidorRepository.GetAllConvertidorAync();
        foreach (var item in conversiones)
        {
            ListaConversiones.Add(item);
        }
    }

    [RelayCommand]
    public async Task CrearConvertidorAsync()
    {
        var nuevo = new Convertidor
        {
            ValorMin = ValorMin,
            ValorMax = ValorMax,
            NumeroTicket = NumeroTicket,
            EstadoConvertidor = EstadoConvertidor
        };
        await _convertidorRepository.CreateConvertidorAsync(nuevo);
        await _alertaHelper.ShowSuccessAsync("Convertidor creado correctamente.");
        await Shell.Current.GoToAsync(".."); // Volver a la página anterior
    }
}
