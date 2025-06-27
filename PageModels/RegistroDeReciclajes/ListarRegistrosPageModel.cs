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
    private string _dniBuscado;

    [ObservableProperty]
    private string mensajeBusqueda;

    private List<RegistroDeReciclaje> _todosLosRegistros = new(); // Almacenamos todos los registros originales

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
        _todosLosRegistros.Clear();

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
                item.ApellidoResidente = residente.ApellidoResidente;
                item.DniResidente = residente.DniResidente;
            }

            if (residuosDict.TryGetValue(item.IdResiduo, out var residuo))
            {
                item.NombreResiduo = residuo.NombreResiduo;
            }

            _todosLosRegistros.Add(item); // Guardamos todos los registros para poder filtrar
        }

        // Cargamos todos los registros por defecto
        foreach (var r in _todosLosRegistros)
        {
            ListaRegistrosResiduo.Add(r);
        }

        MensajeBusqueda = string.Empty;
    }

    [RelayCommand]
    public void FiltrarPorDni()
    {
        try
        {
            // Validación de lista cargada
            if (_todosLosRegistros == null || _todosLosRegistros.Count == 0)
            {
                MensajeBusqueda = "No hay datos cargados para filtrar.";
                return;
            }

            ListaRegistrosResiduo.Clear();

            if (string.IsNullOrWhiteSpace(DniBuscado))
            {
                foreach (var r in _todosLosRegistros)
                    ListaRegistrosResiduo.Add(r);

                MensajeBusqueda = string.Empty;
                return;
            }

            var filtrados = _todosLosRegistros
                .Where(r => !string.IsNullOrEmpty(r.DniResidente) && r.DniResidente.Contains(DniBuscado))
                .ToList();

            foreach (var r in filtrados)
                ListaRegistrosResiduo.Add(r);

            MensajeBusqueda = filtrados.Count == 0
                ? $"No se encontraron registros con el DNI \"{DniBuscado}\"."
                : string.Empty;
        }
        catch (Exception ex)
        {
            MensajeBusqueda = "Error inesperado en búsqueda: " + ex.Message;
        }
    }


}
