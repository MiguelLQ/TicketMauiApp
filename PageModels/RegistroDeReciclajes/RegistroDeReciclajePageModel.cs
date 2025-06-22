//Actualizado
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
       // private readonly IResidenteRepository _residenteRepository;
        private readonly ICategoriaRepository _residuoRepository;

        public ObservableCollection<RegistroDeReciclaje> Registros { get; set; } = new();
        public ObservableCollection<Residuo> ListaResiduos { get; set; } = new();

        private string _dniResidente;
        public string DniResidente
        {
            get => _dniResidente;
            set { _dniResidente = value; OnPropertyChanged(); }
        }

        //private Residente _residenteEncontrado;
        //public Residente ResidenteEncontrado
        //{
        //    get => _residenteEncontrado;
        //    set { _residenteEncontrado = value; OnPropertyChanged(); }
        //}

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

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        // Comandos
        public ICommand BuscarResidenteCommand { get; }
        public ICommand LoadRegistrosCommand { get; }
        public ICommand AddRegistroCommand { get; }
        public ICommand DeleteRegistroCommand { get; }

        // poner IResidenteRepository residenteRepository,
        public RegistroDeReciclajePageModel(IRegistroDeReciclajeRepository registroRepository,
                                            ICategoriaRepository residuoRepository)
        {
            _registroRepository = registroRepository;
         //   _residenteRepository = residenteRepository;
            _residuoRepository = residuoRepository;

            LoadRegistrosCommand = new Command(async () => await LoadRegistrosAsync());
            BuscarResidenteCommand = new Command(async () => await BuscarResidenteAsync());
            AddRegistroCommand = new Command(async () => await AddRegistroAsync());
            DeleteRegistroCommand = new Command<int>(async id => await DeleteRegistroAsync(id));

            _ = LoadRegistrosAsync();
            _ = CargarResiduosAsync();
        }

        private async Task CargarResiduosAsync()
        {
            var residuos = await _residuoRepository.GetAllResiduoAync();
            ListaResiduos.Clear();
            foreach (var residuo in residuos)
                ListaResiduos.Add(residuo);
        }

        private async Task BuscarResidenteAsync()
        {
            if (string.IsNullOrWhiteSpace(DniResidente))
            {
                await AppShell.DisplayToastAsync("Ingresa un DNI válido.");
                return;
            }

            //var residente = await _residenteRepository.ObtenerPorDniAsync(DniResidente);
            //if (residente != null)
            //{
            //    ResidenteEncontrado = residente;
            //    await AppShell.DisplayToastAsync($"Residente encontrado: {residente.NombreResidente}");
            //}
            //else
            //{
            //    await AppShell.DisplayToastAsync("Residente no encontrado.");
            //}
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
            //if (ResidenteEncontrado == null)
            //{
            //    await AppShell.DisplayToastAsync("Debes buscar y seleccionar un residente válido.");
            //    return;
            //}

            if (ResiduoSeleccionado == null)
            {
                await AppShell.DisplayToastAsync("Selecciona un residuo.");
                return;
            }

            var nuevoRegistro = new RegistroDeReciclaje
            {
             //   IdResidente = int.Parse(ResidenteEncontrado.IdResidente),
                IdResiduo = ResiduoSeleccionado.IdResiduo,
                PesoKilogramo = PesoKilogramo,
                TicketsGanados = TicketsGanados,
                FechaRegistro = DateTime.Now
            };

            await _registroRepository.GuardarAsync(nuevoRegistro);

            // Sumar tickets al residente
         //   ResidenteEncontrado.TicketsTotalesGanados += TicketsGanados;
         //   await _residenteRepository.GuardarAsync(ResidenteEncontrado);

            await LoadRegistrosAsync();
            LimpiarFormulario();
        }

        private async Task DeleteRegistroAsync(int id)
        {
            await _registroRepository.EliminarAsync(id);
            await LoadRegistrosAsync();
        }

        private void LimpiarFormulario()
        {
            DniResidente = string.Empty;
        //    ResidenteEncontrado = null;
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
