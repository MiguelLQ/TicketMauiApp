using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;

namespace MauiFirebase.PageModels.Canjes
{
    public class CanjePageModel : INotifyPropertyChanged
    {

        private readonly ICanjeRepository _repository;
        private readonly IPremioRepository _premioRepository;
        public ObservableCollection<Models.Premio> ListaPremios { get; } = new();

        public ObservableCollection<Models.Canje> Canjes { get; set; } = new();

        private string _nombreCanje;
        public string NombreCanje
        {
            get => _nombreCanje;
            set { _nombreCanje = value; OnPropertyChanged(); }
        }

        private int _idPremio;
        public int IdPremio
        {
            get => _idPremio;
            set { _idPremio = value; OnPropertyChanged(); }
        }
        // capturar Id Conticket Seleccionado
        private Models.Premio? _premioSeleccionado;
        public Models.Premio? premioSeleccionado
        {
            get => _premioSeleccionado;
            set
            {
                _premioSeleccionado = value;
                IdPremio = value?.Id ?? 0; // id no puede ser igual a 0
                OnPropertyChanged();
            }
        }


        private bool _estadoCanje = true;
        public bool EstadoCanje
        {
            get => _estadoCanje;
            set { _estadoCanje = value; OnPropertyChanged(); }
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
       

        public CanjePageModel(ICanjeRepository repository, IPremioRepository premioRepository)
        {
            _repository = repository;
            _premioRepository = premioRepository;

            AddCommand = new Command(async () => await AddAsync());
            LoadCommand = new Command(async () => await LoadAsync());
            ChangeEstadoCommand = new Command<int>(async (id) => await CambiarEstado(id));

            // Cargar tickets al inicio
            _ = CargarPremioAsync();
            _ = LoadAsync();
        }
        public async Task CargarPremioAsync()
        {
            try
            {
                var lista = await _premioRepository.ObtenerTodosAsync();
                ListaPremios.Clear();
                foreach (var t in lista)
                    ListaPremios.Add(t);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error cargando Premios: {ex.Message}");
            }
        }

        public async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Canjes.Clear();
                var lista = await _repository.GetAllCanjeAync();
                foreach (var c in lista)
                    Canjes.Add(c);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddAsync()
        {

            var nuevo = new Models.Canje
            {
                FechaCanje = DateTime.Now,
                EstadoCanje = EstadoCanje,
                IdPremio = IdPremio
            };

            await _repository.CreateCanjeAsync(nuevo);
            await LoadAsync();

            EstadoCanje = true;
        }

        private async Task CambiarEstado(int id)
        {
            await _repository.ChangeEstadoCanjeAsync(id);
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
