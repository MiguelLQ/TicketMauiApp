using CommunityToolkit.Mvvm.ComponentModel;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Logins
{
    public class TarjetaResumen
    {
        public string? Titulo { get; set; }
        public string? Valor { get; set; }
        public string? Icono { get; set; } // Usa imágenes o emojis
        public string? Emoji { get; set; } // nuevo campo
    }

    public class DashboardPageModel : ObservableObject
    {
        private readonly IResidenteRepository _residenteRepository;
        private readonly IRegistroDeReciclajeRepository _registroDeReciclajeRepository;
        private readonly IPremioRepository _premioRepository;
        private readonly ITicketRepository _ticketRepository;

        public ObservableCollection<TarjetaResumen> TarjetasResumen { get; set; } = new();

        public DashboardPageModel(IResidenteRepository residenteRepository, IRegistroDeReciclajeRepository registroDeReciclajeRepository, IPremioRepository premioRepository, ITicketRepository ticketRepository)
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
            {
                TarjetasResumen.Add(tarjeta);
            }
        }

        private async Task<List<TarjetaResumen>> ObtenerTarjetasAsync()
        {
            int totalResidentes = await _residenteRepository.TotalResidentes();
            decimal totalReciclado = await _registroDeReciclajeRepository.ObtenerTotalRecicladoKg();
            //int totalTickets = await _ticketRepository.ObtenerCantidadTickets();
            int totalPremios = await _premioRepository.ObtenerCantidadPremios();
            return new List<TarjetaResumen>
            {
                new() { Titulo = "Residentes", Valor = totalResidentes.ToString(), Emoji = "👥" },
                new() { Titulo = "Reciclaje", Valor = $"{totalReciclado} kg", Emoji = "♻️" },
                //new() { Titulo = "Tickets", Valor = totalTickets.ToString(), Emoji = "🎟️" },
                new() { Titulo = "Premios", Valor = totalPremios.ToString(), Emoji = "🏆" }
            };
        }
    }
}
