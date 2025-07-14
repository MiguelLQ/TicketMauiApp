using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Pages.Ticket;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiFirebase.PageModels.Ticket
{
    public class TicketPageModel : INotifyPropertyChanged
    {
        private IClosePopup? _popupCloser; // 🔧 Instancia UI del popup actual
        private readonly ITicketRepository _ticketRepository;
        private readonly IAlertaHelper _alertaHelper;
        private readonly SincronizacionFirebaseService _sincronizacionFirebaseService;

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
                    ColorTicket = value.ColorTicket!;
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

        public TicketPageModel(ITicketRepository ticketRepository, IAlertaHelper alertaHelper, SincronizacionFirebaseService sincronizacionFirebaseService)
        {
            _ticketRepository = ticketRepository;
            _alertaHelper = alertaHelper;

            LoadTicketsCommand = new Command(async () => await LoadTicketsAsync());
            AddTicketCommand = new Command(async () => await AddTicketAsync());
            ChangeEstadoCommand = new Command<string>(async id => await ChangeEstadoAsync(id));
            _sincronizacionFirebaseService = sincronizacionFirebaseService;
            EditTicketCommand = new Command<Models.Ticket>(async ticket => await OnEditTicket(ticket));

            _ = LoadTicketsAsync();
        }

        public async Task LoadTicketsAsync()
        {
            try
            {
                IsBusy = true;
                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    await _sincronizacionFirebaseService!.SincronizarTicketsDesdeFirebaseAsync();

                }
                Tickets.Clear();
                var tickets = await _ticketRepository.GetAllTicketAync();
                foreach (var t in tickets)
                {
                    Tickets.Add(t);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
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
            if (string.IsNullOrWhiteSpace(ColorTicket))
            {
                await _alertaHelper.ShowErrorAsync("El color del ticket es obligatorio.");
                return;
            }

            var nuevo = new Models.Ticket
            {
                ColorTicket = ColorTicket,
                EstadoTicket = EstadoTicket,
                FechaRegistro = DateTime.Now,
                Sincronizado = false
            };

            await _ticketRepository.CreateTicketAsync(nuevo);
            await LoadTicketsAsync();
            LimpiarFormulario();

            _popupCloser?.ClosePopup();
            await _alertaHelper.ShowSuccessAsync("Ticket agregado correctamente.");

            await IntentarSincronizarAsync();
        }

        private async Task IntentarSincronizarAsync()
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await _sincronizacionFirebaseService.SincronizarTicketsAsync();
                }
                catch
                {
                    await _alertaHelper.ShowWarningAsync("Cambios guardados localmente. Se sincronizarán cuando haya conexión.");
                }
            }
        }

        private async Task AddTicketAsync()
        {
            if (string.IsNullOrWhiteSpace(ColorTicket))
            {
                await _alertaHelper.ShowErrorAsync("El color del ticket es obligatorio.");
                return;
            }
            var nuevo = new Models.Ticket
            {
                ColorTicket = ColorTicket,
                EstadoTicket = EstadoTicket,
                FechaRegistro = DateTime.Now
            };

            await _ticketRepository.CreateTicketAsync(nuevo);
            await LoadTicketsAsync();
            LimpiarFormulario();
            await _alertaHelper.ShowSuccessAsync("Ticket agregado correctamente.");

        }

        private async Task GuardarCambiosTicketAsync()
        {
            if (TicketSeleccionado == null) return;

            TicketSeleccionado.ColorTicket = ColorTicket;
            TicketSeleccionado.EstadoTicket = EstadoTicket;
            TicketSeleccionado.Sincronizado = false;

            await _ticketRepository.UpdateTicketAsync(TicketSeleccionado);
            await LoadTicketsAsync();

            _popupCloser?.ClosePopup();
            await _alertaHelper.ShowSuccessAsync("Ticket editado correctamente.");

            await IntentarSincronizarAsync();
        }


        private async Task ChangeEstadoAsync(string id)
        {
            await _ticketRepository.ChangeEstadoTicketAsync(id);
            await LoadTicketsAsync();
            await _alertaHelper.ShowSuccessAsync("Estado cambiado correctamente.");

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
