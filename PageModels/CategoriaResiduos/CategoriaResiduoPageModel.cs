using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiFirebase.Data.Interfaces;

namespace MauiFirebase.PageModels.CategoriaResiduos
{
    public class CategoriaResiduoPageModel : INotifyPropertyChanged
    {
        private readonly ICategoriaResiduoRepository _repository;
        private readonly ITicketRepository _ticketRepository;
        public ObservableCollection<Models.Ticket> ListaTickets { get; } = new();

        public ObservableCollection<Models.CategoriaResiduo> Categorias { get; set; } = new();

        private string _nombreCategoria;
        public string NombreCategoria
        {
            get => _nombreCategoria;
            set { _nombreCategoria = value; OnPropertyChanged(); }
        }

        private int _idTicket;
        public int IdTicket
        {
            get => _idTicket;
            set { _idTicket = value; OnPropertyChanged(); }
        }
        // capturar Id Conticket Seleccionado
        private Models.Ticket? _ticketSeleccionado;
        public Models.Ticket? TicketSeleccionado
        {
            get => _ticketSeleccionado;
            set
            {
                _ticketSeleccionado = value;
                IdTicket = value?.IdTicket ?? 0; // id no puede ser igual a 0
                OnPropertyChanged();
            }
        }


        private bool _estadoCategoriaResiduo = true;
        public bool EstadoCategoriaResiduo
        {
            get => _estadoCategoriaResiduo;
            set { _estadoCategoriaResiduo = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        // Comandos
        public ICommand AddCommand { get; }
        public ICommand LoadCommand { get; }

        //public ICommand LoadCommand { get; }
        public ICommand ChangeEstadoCommand { get; }

        public CategoriaResiduoPageModel(ICategoriaResiduoRepository repository, ITicketRepository ticketRepository)
        {
            _repository = repository;
            _ticketRepository = ticketRepository;

            AddCommand = new Command(async () => await AddAsync());
            LoadCommand = new Command(async() => await LoadAsync());
            ChangeEstadoCommand = new Command<int>(async (id) => await CambiarEstado(id));

            // Cargar tickets al inicio
            _ = CargarTicketsAsync();
            _ = LoadAsync();
        }
        public async Task CargarTicketsAsync()
        {
            try
            {
                var lista = await _ticketRepository.GetAllTicketAync();
                ListaTickets.Clear();
                foreach (var t in lista)
                    ListaTickets.Add(t);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cargando tickets: {ex.Message}");
            }
        }

        public async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Categorias.Clear();
                var lista = await _repository.GetAllCategoriaResiduoAsync();
                foreach (var c in lista)
                    Categorias.Add(c);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreCategoria)) return;

            var nuevo = new Models.CategoriaResiduo
            {
                NombreCategoria = NombreCategoria,
                EstadoCategoriaResiduo = EstadoCategoriaResiduo,
                IdTicket = IdTicket
            };

            await _repository.CreateCategoriaResiduoAsync(nuevo);
            await LoadAsync();

            NombreCategoria = string.Empty;
            EstadoCategoriaResiduo = true;
        }

        private async Task CambiarEstado(int id)
        {
            await _repository.ChangeEstadoCategoriaResiduoAsync(id);
            await LoadAsync();
        }
        // =================== INOTIFY =========================
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
