using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
namespace MauiFirebase.PageModels.RegistroDeReciclajes;

public partial class AgregarRegistroPageModel : ObservableObject
{
    private readonly IRegistroDeReciclajeRepository _registroRepository;
    private readonly IResidenteRepository _residenteRepository;
    private readonly IResiduoRepository _residuoRepository;
    private readonly IAlertaHelper _alertaHelper;
    public ObservableCollection<Residuo> ListaResiduos { get; } = new();
    public ObservableCollection<Residente> ListaResidentes { get; } = new();
    public ObservableCollection<RegistroDeReciclaje> ListaRegistroReciclaje { get; } = new();

    [ObservableProperty]
    private Residente? _residenteSeleccionado;

    [ObservableProperty]
    private Residuo? _residuoSeleccionado;

    [ObservableProperty]
    private decimal _pesoKilogramo;

    [ObservableProperty]
    private int _ticketsGanados;


    public AgregarRegistroPageModel(IRegistroDeReciclajeRepository registroRepository,
                                    IResidenteRepository residenteRepository,
                                    IResiduoRepository residuoRepository,
                                    IAlertaHelper alertaHelper)
    {
        _registroRepository = registroRepository;
        _residenteRepository = residenteRepository;
        _residuoRepository = residuoRepository;
        _alertaHelper = alertaHelper;
    }

    [RelayCommand]

    public async Task CargarResiduoAsync()
    {
        ListaResiduos.Clear();
        var residuo = await _residuoRepository.GetAllResiduoAync();
        foreach (var item in residuo)
        {
            ListaResiduos.Add(item);
        }
    }

    [RelayCommand]
    public async Task CargarResidenteAsync()
    {
        ListaResidentes.Clear();
        var residente = await _residenteRepository.GetAllResidentesAsync();
        foreach (var item in residente)
        {
            ListaResidentes.Add(item);
        }
    }

    [RelayCommand]
    public async Task CargarRegistroReciclajeAsync()
    {
        ListaRegistroReciclaje.Clear();
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
            ListaRegistroReciclaje.Add(item);
        }
    }


    public async Task AddRegistroAsync()
    {
        
        var nuevoRegistro = new RegistroDeReciclaje
        {
            IdResidente = ResidenteSeleccionado?.IdResidente ?? 0,
            IdResiduo = ResiduoSeleccionado?.IdResiduo ?? 0,
            PesoKilogramo = PesoKilogramo,
            TicketsGanados = TicketsGanados,
            FechaRegistro = DateTime.Now
        };

        await _registroRepository.GuardarAsync(nuevoRegistro);

        ResidenteSeleccionado!.TicketsTotalesGanados += TicketsGanados;
        await _residenteRepository.GuardarAsync(ResidenteSeleccionado);

        LimpiarFormulario();
        await _alertaHelper.ShowSuccessAsync("Registro guardado correctamente.");
        await Shell.Current.GoToAsync("..");
    }

    private void LimpiarFormulario()
    {
        ResiduoSeleccionado = null;
        PesoKilogramo = 0;
        TicketsGanados = 0;
    }
}
