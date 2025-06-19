using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;

namespace MauiFirebase.PageModels.RegistroDeReciclajePageModel
{
    public class RegistroDeReciclajePageModel : INotifyPropertyChanged
    {
        private readonly IRegistroDeReciclajeRepository _registroRepository;

        public ObservableCollection<RegistroDeReciclaje> Registros { get; set; } = new();

        private int _idResidente;
        public int IdResidente
        {
            get => _idResidente;
            set { _idResidente = value; OnPropertyChanged(); }
        }

        private int _idResiduo;
        public int IdResiduo
        {
            get => _idResiduo;
            set { _idResiduo = value; OnPropertyChanged(); }
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

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        // Comandos
        public ICommand LoadRegistrosCommand { get; }
        public ICommand AddRegistroCommand { get; }
        public ICommand UpdateRegistroCommand { get; }
        public ICommand DeleteRegistroCommand { get; }

        public RegistroDeReciclajePageModel(IRegistroDeReciclajeRepository registroRepository)
        {
            _registroRepository = registroRepository;

            LoadRegistrosCommand = new Command(async () => await LoadRegistrosAsync());
            AddRegistroCommand = new Command(async () => await AddRegistroAsync());
            UpdateRegistroCommand = new Command<RegistroDeReciclaje>(async registro => await UpdateRegistroAsync(registro));
            DeleteRegistroCommand = new Command<int>(async id => await DeleteRegistroAsync(id));

            _ = LoadRegistrosAsync(); // Carga automática
        }

        public async Task LoadRegistrosAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Registros.Clear();
                var registros = await _registroRepository.ObtenerTodosAsync();
                foreach (var r in registros)
                    Registros.Add(r);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddRegistroAsync()
        {
            var nuevoRegistro = new RegistroDeReciclaje
            {
                IdResidente = IdResidente,
                IdResiduo = IdResiduo,
                PesoKilogramo = PesoKilogramo,
                TicketsGanados = TicketsGanados,
                FechaRegistro = DateTime.Now
            };

            await _registroRepository.GuardarAsync(nuevoRegistro);
            await LoadRegistrosAsync();

            LimpiarFormulario();
        }

        private async Task UpdateRegistroAsync(RegistroDeReciclaje registro)
        {
            registro.IdResidente = IdResidente;
            registro.IdResiduo = IdResiduo;
            registro.PesoKilogramo = PesoKilogramo;
            registro.TicketsGanados = TicketsGanados;

            await _registroRepository.GuardarAsync(registro);
            await LoadRegistrosAsync();
        }

        private async Task DeleteRegistroAsync(int id)
        {
            await _registroRepository.EliminarAsync(id);
            await LoadRegistrosAsync();
        }
        //hbjasvhgsagvhas
        private void LimpiarFormulario()
        {
            IdResidente = 0;
            IdResiduo = 0;
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
