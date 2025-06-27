using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.RegistroDeReciclajes;

public partial class ListarRegistrosPageModel : ObservableObject
{
    public ObservableCollection<RegistroDeReciclaje> ListaRegistrosResiduo { get; } = new();
    public ObservableCollection<Residente> ListaResidentes { get; } = new();
    public ObservableCollection<Residuo> ListaResiduos { get; } = new();

    [ObservableProperty]
    private string _dniBuscado = string.Empty;

    [ObservableProperty]
    private string _mensajeBusqueda = string.Empty;

    [ObservableProperty]
    private bool _mostrarMensaje = false;

    private List<RegistroDeReciclaje> _todosLosRegistros = new();

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
        if (residuos != null)
        {
            foreach (var item in residuos)
            {
                ListaResiduos.Add(item);
            }
        }
    }

    [RelayCommand]
    public async Task CargarResidentesAsync()
    {
        ListaResidentes.Clear();
        var residentes = await _residenteRepository.GetAllResidentesAsync();
        if (residentes != null)
        {
            foreach (var item in residentes)
            {
                ListaResidentes.Add(item);
            }
        }
    }

    [RelayCommand]
    public async Task CargarRegistroResiduoAsync()
    {
        ListaRegistrosResiduo.Clear();
        _todosLosRegistros.Clear();

        var registros = await _registroRepository.ObtenerTodosAsync();
        var residentes = await _residenteRepository.GetAllResidentesAsync();
        var residuos = await _residuoRepository.GetAllResiduoAync();

        if (registros != null && residentes != null && residuos != null)
        {
            var residentesDict = residentes.ToDictionary(r => r.IdResidente);
            var residuosDict = residuos.ToDictionary(r => r.IdResiduo);

            foreach (var item in registros)
            {
                if (residentesDict.TryGetValue(item.IdResidente, out var residente))
                {
                    item.NombreResidente = residente.NombreResidente;
                    item.ApellidoResidente = residente.ApellidoResidente;
                    item.DniResidente = residente.DniResidente;
                }

                if (residuosDict.TryGetValue(item.IdResiduo, out var residuo))
                {
                    item.NombreResiduo = residuo.NombreResiduo;
                }

                _todosLosRegistros.Add(item);
            }

            foreach (var r in _todosLosRegistros)
            {
                ListaRegistrosResiduo.Add(r);
            }
        }

        MensajeBusqueda = string.Empty;
        MostrarMensaje = false;
    }

    [RelayCommand]
    public void FiltrarPorDni()
    {
        try
        {
            if (_todosLosRegistros == null || _todosLosRegistros.Count == 0)
            {
                MensajeBusqueda = "No hay datos cargados para filtrar.";
                MostrarMensaje = true;
                return;
            }

            ListaRegistrosResiduo.Clear();

            if (string.IsNullOrWhiteSpace(DniBuscado))
            {
                foreach (var r in _todosLosRegistros)
                    ListaRegistrosResiduo.Add(r);

                MensajeBusqueda = string.Empty;
                MostrarMensaje = false;
                return;
            }

            var filtrados = _todosLosRegistros
                .Where(r => r.DniResidente != null && r.DniResidente.Contains(DniBuscado, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var r in filtrados)
                ListaRegistrosResiduo.Add(r);

            if (filtrados.Count == 0)
            {
                MensajeBusqueda = $"No se encontraron registros con el DNI \"{DniBuscado}\".";
                MostrarMensaje = true;
            }
            else
            {
                MensajeBusqueda = string.Empty;
                MostrarMensaje = false;
            }
        }
        catch (Exception ex)
        {
            MensajeBusqueda = "Error inesperado en búsqueda: " + ex.Message;
            MostrarMensaje = true;
        }
    }
}
