using CommunityToolkit.Mvvm.ComponentModel;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;

namespace MauiFirebase.PageModels.Logins
{
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

        // ▶ Colecciones para la vista
        public ObservableCollection<TarjetaResumen> TarjetasResumen { get; } = new();
        public ObservableCollection<RegistroRecienteViewModel> UltimosRegistrosResumen { get; } = new();
        public ObservableCollection<ReciclajePorCategoria> DatosGrafico { get; } = new();
        public ObservableCollection<Vehiculo> VehiculosHoy { get; } = new();

        // ▶ Gráfico de pastel
        [ObservableProperty] private Chart? _graficoPastel;

        public DashboardPageModel(
            IResidenteRepository residenteRepository,
            IRegistroDeReciclajeRepository reciclajeRepository,
            IPremioRepository premioRepository,
            IResiduoRepository residuoRepository,
            ITicketRepository ticketRepository,
            IVehiculoRepository vehiculoRepository)
        {
            _residenteRepo = residenteRepository;
            _reciclajeRepo = reciclajeRepository;
            _premioRepo = premioRepository;
            _ticketRepo = ticketRepository;
            _residuoRepo = residuoRepository;
            _vehiculoRepo = vehiculoRepository;
        }

        // ════════════════════════════════════════════════════════════
        //  MÉTODO PRINCIPAL
        // ════════════════════════════════════════════════════════════
        public async Task InicializarAsync()
        {
            // Tarjetas
            TarjetasResumen.Clear();
            foreach (var t in await ObtenerTarjetasAsync())
                TarjetasResumen.Add(t);

            // Últimos registros
            await CargarUltimosRegistrosAsync();

            // Gráfico pastel
            await CargarGraficoPastelAsync();

            // Vehículos con ruta hoy
            await CargarVehiculosHoyAsync();
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
                string borde = "#2196F3";
                string texto = "#0D47A1";
                string desc = $"{reg.NombreResidente} recicló {reg.PesoKilogramo} kg de {reg.NombreResiduo}";

                if ((reg.NombreResiduo ?? "").ToLower().Contains("papel"))
                { icono = "papel.png"; borde = "#8BC34A"; texto = "#33691E"; }
                else if ((reg.NombreResiduo ?? "").ToLower().Contains("vidrio"))
                { icono = "vidrio.png"; borde = "#FF9800"; texto = "#E65100"; }

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
                LabelTextSize = 30,
                BackgroundColor = SKColors.Transparent
            };
        }

        // ════════════════════════════════════════════════════════════
        //  VEHÍCULOS DEL DÍA
        // ════════════════════════════════════════════════════════════
        private async Task CargarVehiculosHoyAsync()
        {
            var hoy = DateTime.Today.DayOfWeek;
            var lista = await _vehiculoRepo.ObtenerVehiculosPorDiaAsync(hoy);

            VehiculosHoy.Clear();
            foreach (var v in lista)
                VehiculosHoy.Add(v);
        }

        // ════════════════════════════════════════════════════════════
        private string GenerarColorAleatorioHex()
        {
            var rnd = new Random();
            return $"#{rnd.Next(0x1000000):X6}";
        }
    }
}
