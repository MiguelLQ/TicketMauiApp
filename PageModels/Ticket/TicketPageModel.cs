using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiFirebase.Data.Interfaces;

namespace MauiFirebase.PageModels.Ticket
{
    public class TicketPageModel : INotifyPropertyChanged
    {
        private readonly ITicketRepository _ticketRepository;

        public ObservableCollection<Models.Ticket> Tickets { get; set; } = new();

        private string _colorTicket;
        public string ColorTicket
        {
            get => _colorTicket;
            set { _colorTicket = value; OnPropertyChanged(); }
        }

        private bool _estadoTicket = true; // por defecto en true
        public bool EstadoTicket
        {
            get => _estadoTicket;
            set
            {
                _estadoTicket = value;
                OnPropertyChanged();
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoadTicketsCommand { get; }
        public ICommand AddTicketCommand { get; }
        public ICommand ChangeEstadoCommand { get; }

        public TicketPageModel(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;

            LoadTicketsCommand = new Command(async () => await LoadTicketsAsync());
            AddTicketCommand = new Command(async () => await AddTicketAsync());
            ChangeEstadoCommand = new Command<int>(async id => await ChangeEstadoAsync(id));

            _ = LoadTicketsAsync(); // 👈 Se cargan los tickets automáticamente
        }

        public async Task LoadTicketsAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Tickets.Clear();
                var tickets = await _ticketRepository.GetAllTicketAync();
                foreach (var t in tickets)
                    Tickets.Add(t);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddTicketAsync()
        {
            // Solo validamos el ColorTicket, ya que EstadoTicket es bool
            if (string.IsNullOrWhiteSpace(ColorTicket))
                return;

            var newTicket = new Models.Ticket
            {
                ColorTicket = ColorTicket,
                EstadoTicket = EstadoTicket, // 👈 esto ya es un bool (true/false)
                FechaRegistro = DateTime.Now
            };

            await _ticketRepository.CreateTicketAsync(newTicket);
            await LoadTicketsAsync();

            ColorTicket = string.Empty;
            EstadoTicket = true; // reiniciamos a estado por defecto si quieres
        }


        private async Task ChangeEstadoAsync(int id)
        {
            await _ticketRepository.ChangeEstadoTicketAsync(id);
            await LoadTicketsAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
