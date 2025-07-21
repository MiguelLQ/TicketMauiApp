using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MauiFirebase.PageModels.Logins;

public class TarjetaResumen
{
    public string? Titulo { get; set; }
    public string? Valor { get; set; }
    public string? Emoji { get; set; }
}

public class RegistroRecienteViewModel
{
    public string? Icono { get; set; }
    public string? Descripcion { get; set; }
    public string? ColorBorde { get; set; }
    public string? ColorTexto { get; set; }
}

public partial class DashboardPageModel : ObservableObject
{
    private readonly IResidenteRepository _residenteRepo;
    private readonly IRegistroDeReciclajeRepository _reciclajeRepo;
    private readonly IPremioRepository _premioRepo;
    private readonly ITicketRepository _ticketRepo;
    private readonly IResiduoRepository _residuoRepo;
    private readonly IVehiculoRepository _vehiculoRepo;
    private readonly SincronizacionFirebaseService _sincronizador;

    // ▶ Colecciones para la vista
    public ObservableCollection<TarjetaResumen> TarjetasResumen { get; } = new();
    public ObservableCollection<RegistroRecienteViewModel> UltimosRegistrosResumen { get; } = new();
    public ObservableCollection<ReciclajePorCategoria> DatosGrafico { get; } = new();
    public ObservableCollection<Vehiculo> VehiculosHoy { get; } = new();
    [ObservableProperty]
    bool isBusy;


    // ▶ Gráfico de pastel
    [ObservableProperty] private Chart? _graficoPastel;
    private static bool _yaSincronizo = false;

    public DashboardPageModel(
        IResidenteRepository residenteRepository,
        IRegistroDeReciclajeRepository reciclajeRepository,
        IPremioRepository premioRepository,
        IResiduoRepository residuoRepository,
        SincronizacionFirebaseService sincronizador,
        ITicketRepository ticketRepository,
        IVehiculoRepository vehiculoRepository)
    {
        _residenteRepo = residenteRepository;
        _reciclajeRepo = reciclajeRepository;
        _premioRepo = premioRepository;
        _ticketRepo = ticketRepository;
        _residuoRepo = residuoRepository;
        _vehiculoRepo = vehiculoRepository;
        _sincronizador = sincronizador;
    }

    // ════════════════════════════════════════════════════════════
    //  MÉTODO PRINCIPAL
    // ════════════════════════════════════════════════════════════
    public async Task InicializarAsync()
    {
        if (IsBusy) return; // Previene múltiples llamadas
        try
        {
            IsBusy = true;

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _sincronizador.SincronizarPremiosAsync();
                await _sincronizador.SincronizarResidentesDesdeFirebaseAsync();
            }

            if (!_yaSincronizo)
            {
                await _sincronizador.SincronizarTodoAsync();
                _yaSincronizo = true;
            }

            // Tarjetas
            TarjetasResumen.Clear();
            foreach (var t in await ObtenerTarjetasAsync())
            {
                TarjetasResumen.Add(t);
            }

            // Últimos registros
            await CargarUltimosRegistrosAsync();

            // Gráfico pastel
            await CargarGraficoPastelAsync();

            // Vehículos con ruta hoy
            await CargarVehiculosHoyAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Error] InicializarAsync: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }


    // ════════════════════════════════════════════════════════════
    //  TARJETAS RESUMEN
    // ════════════════════════════════════════════════════════════
    private async Task<List<TarjetaResumen>> ObtenerTarjetasAsync()
    {
        int totalResidentes = await _residenteRepo.TotalResidentes();
        decimal totalReciclado = await _reciclajeRepo.ObtenerTotalRecicladoKg();
        int totalPremios = await _premioRepo.ObtenerCantidadPremios();

        return new List<TarjetaResumen>
        {
            new() { Titulo = "Ciudadanos", Valor = totalResidentes.ToString(), Emoji = "👥" },
            new() { Titulo = "Reciclaje",  Valor = $"{totalReciclado} kg",      Emoji = "♻️" },
            new() { Titulo = "Premios",    Valor = totalPremios.ToString(),    Emoji = "🏆" }
        };
    }

    // ════════════════════════════════════════════════════════════
    //  ÚLTIMOS 3 REGISTROS DE RECICLAJE
    // ════════════════════════════════════════════════════════════
    public async Task CargarUltimosRegistrosAsync()
    {
        UltimosRegistrosResumen.Clear();

        var registros = await _reciclajeRepo.UltimosTresRegistros();
        var residentes = await _residenteRepo.GetAllResidentesAsync();
        var residuos = await _residuoRepo.GetAllResiduoAync();

        var resiDict = residentes.ToDictionary(r => r.IdResidente);
        var resi2Dict = residuos.ToDictionary(r => r.IdResiduo);

        foreach (var reg in registros)
        {
            if (resiDict.TryGetValue(reg.IdResidente!, out var r1))
                reg.NombreResidente = r1.NombreResidente;

            if (resi2Dict.TryGetValue(reg.IdResiduo!, out var r2))
                reg.NombreResiduo = r2.NombreResiduo;

            // estéticas
            string icono = "plastico.png";
            string borde = "#29303e";
            string texto = GenerarColorAleatorioHex();
            string desc = $"{reg.NombreResidente} recicló {reg.PesoKilogramo} kg de {reg.NombreResiduo}";

            if ((reg.NombreResiduo ?? "").ToLower().Contains("papel"))
            { icono = "papel.png"; borde = "#29303e"; texto = GenerarColorAleatorioHex(); }
            else if ((reg.NombreResiduo ?? "").ToLower().Contains("vidrio"))
            { icono = "vidrio.png"; borde = "#29303e"; GenerarColorAleatorioHex(); }

            UltimosRegistrosResumen.Add(new RegistroRecienteViewModel
            {
                Icono = icono,
                Descripcion = desc,
                ColorBorde = borde,
                ColorTexto = texto
            });
        }
    }

    // ════════════════════════════════════════════════════════════
    //  GRÁFICO DE PASTEL
    // ════════════════════════════════════════════════════════════
    private async Task CargarGraficoPastelAsync()
    {
        var categorias = await _reciclajeRepo.ObtenerTotalesPorCategoriaAsync();

        var entries = categorias.Select(c =>
        {
            var color = SKColor.Parse(GenerarColorAleatorioHex());
            return new ChartEntry((float)c.TotalKg)
            {
                Label = c.Categoria,
                ValueLabel = $"{c.TotalKg} kg",
                Color = color,
                ValueLabelColor = color
            };
        }).ToList();

        GraficoPastel = new PieChart
        {
            Entries = entries,
            LabelTextSize = 20,
            BackgroundColor = SKColors.Transparent
        };
    }

    // ════════════════════════════════════════════════════════════
    //  VEHÍCULOS DEL DÍA
    // ════════════════════════════════════════════════════════════
    private async Task CargarVehiculosHoyAsync()
    {
        var hoy = DateTime.Today.DayOfWeek;
        Debug.WriteLine($"[VehículosHoy] Día actual: {hoy}");

        var lista = await _vehiculoRepo.ObtenerVehiculosPorDiaAsync(hoy);
        Debug.WriteLine($"[VehículosHoy] Vehículos devueltos por repo: {lista.Count}");

        VehiculosHoy.Clear();
        foreach (var v in lista)
        {
            Debug.WriteLine($"   → {v.PlacaVehiculo} | {v.MarcaVehiculo} | {v.Nombre}");
            VehiculosHoy.Add(v);
        }

        Debug.WriteLine($"[VehículosHoy] Total en colección Observable: {VehiculosHoy.Count}");
    }

    // ════════════════════════════════════════════════════════════
    private string GenerarColorAleatorioHex()
    {
        var rnd = new Random();
        return $"#{rnd.Next(0x1000000):X6}";
    }

    /*=================================================================
     * Boton para sincronizar
     =================================================================*/
    [RelayCommand]
    public async Task ForzarSincronizacionAsync()
    {
        await _sincronizador.SincronizarTodoAsync();
        await InicializarAsync(); 
    }
}
