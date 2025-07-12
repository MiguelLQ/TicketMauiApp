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

    public class DashboardPageModel : ObservableObject
    {
        private readonly IResidenteRepository _residenteRepository;
        private readonly IRegistroDeReciclajeRepository _registroDeReciclajeRepository;
        private readonly IPremioRepository _premioRepository;
        private readonly ITicketRepository _ticketRepository;

        public ObservableCollection<TarjetaResumen> TarjetasResumen { get; set; } = new();
        public ObservableCollection<ReciclajePorCategoria> DatosGrafico { get; set; } = new();

        private Chart _graficoPastel;
        public Chart GraficoPastel
        {
            get => _graficoPastel;
            set => SetProperty(ref _graficoPastel, value);
        }

        public DashboardPageModel(
            IResidenteRepository residenteRepository,
            IRegistroDeReciclajeRepository registroDeReciclajeRepository,
            IPremioRepository premioRepository,
            ITicketRepository ticketRepository)
        {
            _residenteRepository = residenteRepository;
            _registroDeReciclajeRepository = registroDeReciclajeRepository;
            _premioRepository = premioRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task InicializarAsync()
        {
            var tarjetas = await ObtenerTarjetasAsync();
            TarjetasResumen.Clear();

            foreach (var tarjeta in tarjetas)
                TarjetasResumen.Add(tarjeta);

            await CargarReciclajePorCategoriaAsync();
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

        private async Task CargarReciclajePorCategoriaAsync()
        {
            var categorias = await _registroDeReciclajeRepository.ObtenerTotalesPorCategoriaAsync();
            DatosGrafico.Clear();

            foreach (var item in categorias)
                DatosGrafico.Add(item);
        }

        private async Task CargarGraficoPastelAsync()
        {
            // 1. Obtén los datos agrupados por categoría
            var categorias = await _registroDeReciclajeRepository.ObtenerTotalesPorCategoriaAsync();

            // 2. Construye las entradas del gráfico, coloreando texto y segmento igual
            var entries = categorias.Select(c =>
            {
                var color = SKColor.Parse(GenerarColorAleatorioHex());

                return new ChartEntry((float)c.TotalKg)
                {
                    Label = c.Categoria,
                    ValueLabel = $"{c.TotalKg} kg",
                    Color = color,            // color del segmento
                    ValueLabelColor = color             // mismo color para el texto “kg”
                                                        // si quieres que el texto sea invisible, usa:  ValueLabelColor = SKColors.Transparent
                };
            }).ToList();

            // 3. Crea el gráfico y asígnalo al binding
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
