using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Models;
using MauiFirebase.Pages.RegistroDeReciclaje;

namespace MauiFirebase.PageModels.RegistroDeReciclajes
{
    public class AgregarRegistroPageModel : INotifyPropertyChanged
    {
        private readonly IRegistroDeReciclajeRepository _registroRepository;
        private readonly IResidenteRepository _residenteRepository;
        private readonly IResiduoRepository _residuoRepository;
        private readonly IAlertaHelper _alertaHelper;
        public ObservableCollection<Residuo> ListaResiduos { get; set; } = new();

        public ICommand AddRegistroCommand { get; }

        private Residente _residenteSeleccionado;
        public Residente ResidenteSeleccionado
        {
            get => _residenteSeleccionado;
            set { _residenteSeleccionado = value; OnPropertyChanged(); }
        }

        private Residuo _residuoSeleccionado;
        public Residuo ResiduoSeleccionado
        {
            get => _residuoSeleccionado;
            set { _residuoSeleccionado = value; OnPropertyChanged(); }
        }

        private decimal _pesoKilogramo;
        public decimal PesoKilogramo
        {
            get => _pesoKilogramo;
            set { _pesoKilogramo = value; OnPropertyChanged(); }
        }

        private int _ticketsGanados;
        public int TicketsGanados
        {
            get => _ticketsGanados;
            set { _ticketsGanados = value; OnPropertyChanged(); }
        }

        public AgregarRegistroPageModel(IRegistroDeReciclajeRepository registroRepository,
                                        IResidenteRepository residenteRepository,
                                        IResiduoRepository residuoRepository,
                                        IAlertaHelper alertaHelper)
        {
            _registroRepository = registroRepository;
            _residenteRepository = residenteRepository;
            _residuoRepository = residuoRepository;
            _alertaHelper = alertaHelper;

            AddRegistroCommand = new Command(async () => await AddRegistroAsync());
            _ = CargarResiduosAsync();
        }

        private async Task CargarResiduosAsync()
        {
            var residuos = await _residuoRepository.GetAllResiduoAync();
            ListaResiduos.Clear();
            foreach (var residuo in residuos)
                ListaResiduos.Add(residuo);
        }

        private async Task AddRegistroAsync()
        {
            if (ResidenteSeleccionado == null || ResiduoSeleccionado == null)
            {
                await _alertaHelper.ShowErrorAsync("Selecciona un residente y un residuo.");
                return;
            }

            var nuevoRegistro = new RegistroDeReciclaje
            {
                IdResidente = ResidenteSeleccionado.IdResidente,
                IdResiduo = ResiduoSeleccionado.IdResiduo,
                PesoKilogramo = PesoKilogramo,
                TicketsGanados = TicketsGanados,
                FechaRegistro = DateTime.Now
            };

            await _registroRepository.GuardarAsync(nuevoRegistro);

            ResidenteSeleccionado.TicketsTotalesGanados += TicketsGanados;
            await _residenteRepository.GuardarAsync(ResidenteSeleccionado);

            LimpiarFormulario();
            await _alertaHelper.ShowSuccessAsync("Registro guardado correctamente.");
            await Shell.Current.GoToAsync(nameof(ListarRegistrosPage));
        }

        private void LimpiarFormulario()
        {
            ResiduoSeleccionado = null;
            PesoKilogramo = 0;
            TicketsGanados = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
