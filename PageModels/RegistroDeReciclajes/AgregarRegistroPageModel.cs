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
    private readonly IConvertidorRepository _convertidorRepository;
    private readonly IAlertaHelper _alertaHelper;

    public ObservableCollection<Residuo> ListaResiduos { get; } = new();
    public ObservableCollection<Residente> ListaResidentes { get; } = new();
    public ObservableCollection<RegistroDeReciclaje> ListaRegistroReciclaje { get; } = new();

    [ObservableProperty]
    private string? _dniBuscado;

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
                                    IConvertidorRepository convertidorRepository,
                                    IAlertaHelper alertaHelper)
    {
        _registroRepository = registroRepository;
        _residenteRepository = residenteRepository;
        _residuoRepository = residuoRepository;
        _convertidorRepository = convertidorRepository;
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

    [RelayCommand]
    public async Task AddRegistroAsync()
    {
        if (ResidenteSeleccionado == null || ResiduoSeleccionado == null || PesoKilogramo <= 0)
        {
            await _alertaHelper.ShowErrorAsync("Completa todos los campos correctamente.");
            return;
        }

        // ✅ 1. Obtener valor del residuo
        int valorResiduo = ResiduoSeleccionado.ValorResiduo;

        // ✅ 2. Calcular valor total
        decimal valorTotal = PesoKilogramo * valorResiduo;

        // ✅ 3. Obtener todos los convertidores activos
        var convertidores = await _convertidorRepository.GetAllConvertidorAync();

        // ✅ 4. Buscar el rango correspondiente
        int ticketsCalculados = 0;
        foreach (var convertidor in convertidores.Where(c => c.EstadoConvertidor))
        {
            if (valorTotal >= convertidor.ValorMin && valorTotal <= convertidor.ValorMax)
            {
                ticketsCalculados = convertidor.NumeroTicket;
                break;
            }
        }

        // ✅ 5. Asignar tickets
        TicketsGanados = ticketsCalculados;

        // ✅ 6. Crear y guardar registro
        var nuevoRegistro = new RegistroDeReciclaje
        {
            IdResidente = ResidenteSeleccionado.IdResidente,
            IdResiduo = ResiduoSeleccionado.IdResiduo,
            PesoKilogramo = PesoKilogramo,
            TicketsGanados = TicketsGanados,
            FechaRegistro = DateTime.Now
        };

        await _registroRepository.GuardarAsync(nuevoRegistro);

        // ✅ 7. Sumar al total del residente
        ResidenteSeleccionado.TicketsTotalesGanados += TicketsGanados;
        await _residenteRepository.GuardarAsync(ResidenteSeleccionado);

        LimpiarFormulario();
        await _alertaHelper.ShowSuccessAsync("Registro guardado correctamente.");
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task BuscarPorDniAsync()
    {
        if (!string.IsNullOrWhiteSpace(DniBuscado))
        {
            var residente = await _residenteRepository.ObtenerPorDniAsync(DniBuscado);
            if (residente != null)
            {
                ResidenteSeleccionado = residente;

                await _alertaHelper.ShowSuccessAsync($"Residente encontrado: {residente.NombreResidente}");
            }
            else
            {
                await _alertaHelper.ShowErrorAsync("Residente no encontrado.");
            }
        }
        else
        {
            await _alertaHelper.ShowErrorAsync("Ingresa un DNI válido.");
            return;
        }
    }

    public void LimpiarFormulario()
    {
        ResiduoSeleccionado = null;
        PesoKilogramo = 0;
        TicketsGanados = 0;
        ResidenteSeleccionado = null;
        DniBuscado = string.Empty;
    }

    // ✅ NUEVO: Detectar cambios automáticos y calcular tickets

    partial void OnPesoKilogramoChanged(decimal oldValue, decimal newValue)
    {
        CalcularTicketsGanados();
    }

    partial void OnResiduoSeleccionadoChanged(Residuo? oldValue, Residuo? newValue)
    {
        CalcularTicketsGanados();
    }

    private async void CalcularTicketsGanados()
    {
        if (ResiduoSeleccionado == null || PesoKilogramo <= 0)
        {
            TicketsGanados = 0;
            return;
        }

        try
        {
            int valorResiduo = ResiduoSeleccionado.ValorResiduo;
            decimal valorTotal = PesoKilogramo * valorResiduo;

            var convertidores = await _convertidorRepository.GetAllConvertidorAync();// 

            int ticketsCalculados = 0;
            if (valorTotal > 400)
            {
                ticketsCalculados = 5;
            }

            else if (valorTotal <= 100)
            {
                ticketsCalculados = 1;
            }

            else
            {
                foreach (var convertidor in convertidores.Where(c => c.EstadoConvertidor))
                {
                    if (valorTotal >= convertidor.ValorMin && valorTotal <= convertidor.ValorMax)
                    {
                        ticketsCalculados = convertidor.NumeroTicket;
                        break;
                    }
                }
            }

            TicketsGanados = ticketsCalculados;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error al calcular tickets: {ex.Message}");
            TicketsGanados = 0;
        }
    }
}
