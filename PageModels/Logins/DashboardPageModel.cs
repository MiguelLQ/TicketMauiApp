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
        public string? Icono { get; set; }
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
        private readonly IResidenteRepository _residenteRepository;
        private readonly IRegistroDeReciclajeRepository _registroDeReciclajeRepository;
        private readonly IPremioRepository _premioRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IResiduoRepository _residuoRepository;

        public ObservableCollection<TarjetaResumen> TarjetasResumen { get; set; } = new();
        public ObservableCollection<RegistroRecienteViewModel> UltimosRegistrosResumen { get; } = new();
        public ObservableCollection<ReciclajePorCategoria> DatosGrafico { get; set; } = new();
        [ObservableProperty]
        private Chart? _graficoPastel;
       
        public DashboardPageModel(
            IResidenteRepository residenteRepository,
            IRegistroDeReciclajeRepository registroDeReciclajeRepository,
            IPremioRepository premioRepository,
            IResiduoRepository residuoRepository,
            ITicketRepository ticketRepository)
        {
            _residenteRepository = residenteRepository;
            _registroDeReciclajeRepository = registroDeReciclajeRepository;
            _premioRepository = premioRepository;
            _ticketRepository = ticketRepository;
            _residuoRepository = residuoRepository;

        }

        public async Task InicializarAsync()
        {
            var tarjetas = await ObtenerTarjetasAsync();
            TarjetasResumen.Clear();

            foreach (var tarjeta in tarjetas)
            {
                TarjetasResumen.Add(tarjeta);
            }
            await CargarUltimosRegistrosAsync();
            await CargarGraficoPastelAsync();
        }

        private async Task<List<TarjetaResumen>> ObtenerTarjetasAsync()
        {
            int totalResidentes = await _residenteRepository.TotalResidentes();
            decimal totalReciclado = await _registroDeReciclajeRepository.ObtenerTotalRecicladoKg();
            int totalPremios = await _premioRepository.ObtenerCantidadPremios();

            return new List<TarjetaResumen>
            {
                new() { Titulo = "Ciudadanos", Valor = totalResidentes.ToString(), Emoji = "👥" },
                new() { Titulo = "Reciclaje", Valor = $"{totalReciclado} kg", Emoji = "♻️" },
                new() { Titulo = "Premios", Valor = totalPremios.ToString(), Emoji = "🏆" }
            };
        }

        public async Task CargarUltimosRegistrosAsync()
        {
            UltimosRegistrosResumen.Clear();
            var registros = await _registroDeReciclajeRepository.UltimosTresRegistros();
            var residentes = await _residenteRepository.GetAllResidentesAsync();
            var residuos = await _residuoRepository.GetAllResiduoAync();
            var residentesDict = residentes.ToDictionary(r => r.IdResidente);
            var residuosDict = residuos.ToDictionary(r => r.IdResiduo);

            foreach (var reg in registros)
            {
                if (residentesDict.TryGetValue(reg.IdResidente!, out var residente))
                {
                    reg.NombreResidente = residente.NombreResidente;
                }
                if (residuosDict.TryGetValue(reg.IdResiduo!, out var residuo))
                {
                    reg.NombreResiduo = residuo.NombreResiduo;
                }
                var icono = "";
                var colorBorde = "#2196F3";
                var colorTexto = "#0D47A1";
                var descripcion = $"{reg.NombreResidente ?? ""} recicló {reg.PesoKilogramo} kg de {reg.NombreResiduo ?? "residuo"}";
                if ((reg.NombreResiduo ?? "").ToLower().Contains("papel"))
                {
                    icono = "papel.png";
                    colorBorde = "#8BC34A";
                    colorTexto = "#33691E";
                }
                else if ((reg.NombreResiduo ?? "").ToLower().Contains("vidrio"))
                {
                    icono = "vidrio.png";
                    colorBorde = "#FF9800";
                    colorTexto = "#E65100";
                }
                else
                {
                    icono = "plastico.png";
                }
                UltimosRegistrosResumen.Add(new RegistroRecienteViewModel
                {
                    Icono = icono,
                    Descripcion = descripcion,
                    ColorBorde = colorBorde,
                    ColorTexto = colorTexto
                });
            }
        }

        private async Task CargarGraficoPastelAsync()
        {
            var categorias = await _registroDeReciclajeRepository.ObtenerTotalesPorCategoriaAsync();
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


        private string GenerarColorAleatorioHex()
        {
            Random random = new();
            return $"#{random.Next(0x1000000):X6}";
        }
    }
}