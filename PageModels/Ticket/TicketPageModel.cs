using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Pages.Ticket;

namespace MauiFirebase.PageModels.Ticket
{
    public class TicketPageModel : INotifyPropertyChanged
    {
        private IClosePopup? _popupCloser; // 🔧 Instancia UI del popup actual
        private readonly ITicketRepository _ticketRepository;

        public ObservableCollection<Models.Ticket> Tickets { get; set; } = new();

        public string ColorTicket
        {
            get => _colorTicket;
            set { _colorTicket = value; OnPropertyChanged(); }
        }
        private string _colorTicket = string.Empty;

        public bool EstadoTicket
        {
            get => _estadoTicket;
            set { _estadoTicket = value; OnPropertyChanged(); }
        }
        private bool _estadoTicket = true;

        public Models.Ticket TicketSeleccionado
        {
            get => _ticketSeleccionado;
            set
            {
                _ticketSeleccionado = value;
                if (value != null)
                {
                    ColorTicket = value.ColorTicket;
                    EstadoTicket = value.EstadoTicket;
                }
                OnPropertyChanged();
            }
        }
        private Models.Ticket _ticketSeleccionado;

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }
        private bool _isBusy;

        // Comandos
        public ICommand LoadTicketsCommand { get; }
        public ICommand AddTicketCommand { get; }
        public ICommand ChangeEstadoCommand { get; }
        public ICommand EditTicketCommand { get; }
        public ICommand GuardarEdicionTicketCommand => new Command(async () => await GuardarCambiosTicketAsync());
        public ICommand MostrarAgregarTicketCommand => new Command(MostrarAgregarPopup);
        public ICommand GuardarNuevoTicketCommand => new Command(async () => await GuardarNuevoTicketAsync());

        public TicketPageModel(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;

            LoadTicketsCommand = new Command(async () => await LoadTicketsAsync());
            AddTicketCommand = new Command(async () => await AddTicketAsync());
            ChangeEstadoCommand = new Command<int>(async id => await ChangeEstadoAsync(id));
            EditTicketCommand = new Command<Models.Ticket>(async ticket => await OnEditTicket(ticket));

            _ = LoadTicketsAsync();
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

        // Establece el popup actual para poder cerrarlo desde el ViewModel
        public void SetPopupCloser(IClosePopup popup)
        {
            _popupCloser = popup;
        }

        private async void MostrarAgregarPopup()
        {
            ColorTicket = string.Empty;
            EstadoTicket = true;
            TicketSeleccionado = null;

            var popup = new AgregarTicketPopup();
            popup.BindingContext = this;
            SetPopupCloser(popup); // << Esto permite luego cerrarlo
            await Application.Current.MainPage.ShowPopupAsync(popup);
        }

        private async Task GuardarNuevoTicketAsync()
        {
            if (string.IsNullOrWhiteSpace(ColorTicket)) return;

            var nuevo = new Models.Ticket
            {
                ColorTicket = ColorTicket,
                EstadoTicket = EstadoTicket,
                FechaRegistro = DateTime.Now
            };

            await _ticketRepository.CreateTicketAsync(nuevo);
            await LoadTicketsAsync();
            LimpiarFormulario();

            _popupCloser?.ClosePopup(); // ✅ Cerramos el popup actual
        }

        private async Task AddTicketAsync()
        {
            if (string.IsNullOrWhiteSpace(ColorTicket)) return;

            var nuevo = new Models.Ticket
            {
                ColorTicket = ColorTicket,
                EstadoTicket = EstadoTicket,
                FechaRegistro = DateTime.Now
            };

            await _ticketRepository.CreateTicketAsync(nuevo);
            await LoadTicketsAsync();
            LimpiarFormulario();
        }

        private async Task GuardarCambiosTicketAsync()
        {
            if (TicketSeleccionado == null) return;

            TicketSeleccionado.ColorTicket = ColorTicket;
            TicketSeleccionado.EstadoTicket = EstadoTicket;

            await _ticketRepository.UpdateTicketAsync(TicketSeleccionado);
            await LoadTicketsAsync();
            _popupCloser?.ClosePopup();
        }

        private async Task ChangeEstadoAsync(int id)
        {
            await _ticketRepository.ChangeEstadoTicketAsync(id);
            await LoadTicketsAsync();
        }

        private async Task OnEditTicket(Models.Ticket ticket)
        {
            if (ticket == null) return;

            TicketSeleccionado = ticket;

            try
            {
                var popup = new EditarTicketPopup();
                popup.BindingContext = this;
                SetPopupCloser(popup);
                await Application.Current.MainPage.ShowPopupAsync(popup);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void LimpiarFormulario()
        {
            ColorTicket = string.Empty;
            EstadoTicket = true;
            TicketSeleccionado = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
