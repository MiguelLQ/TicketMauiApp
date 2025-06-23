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
using MauiFirebase.Models;

namespace MauiFirebase.PageModels.Canjes
{
    public class CanjePageModel : INotifyPropertyChanged
    {
        private readonly ICanjeRepository _repository;
        private readonly IPremioRepository _premioRepository;
        private readonly IResidenteRepository _residenteRepository; // NUEVO agregado

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

        private Models.Premio? _premioSeleccionado;
        public Models.Premio? premioSeleccionado
        {
            get => _premioSeleccionado;
            set
            {
                _premioSeleccionado = value;
                IdPremio = value?.IdPremio ?? 0;
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

        // NUEVO: Para capturar el DNI
        private string _dniResidente;
        public string DniResidente
        {
            get => _dniResidente;
            set { _dniResidente = value; OnPropertyChanged(); }
        }

        // NUEVO: Para guardar el residente encontrado
        private Residente _residenteEncontrado;
        public Residente ResidenteEncontrado
        {
            get => _residenteEncontrado;
            set { _residenteEncontrado = value; OnPropertyChanged(); }
        }

        // Comandos
        public ICommand AddCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand ChangeEstadoCommand { get; }
        public ICommand BuscarResidenteCommand { get; } // NUEVO comando

        public CanjePageModel(ICanjeRepository repository, IPremioRepository premioRepository, IResidenteRepository residenteRepository) // Modificado constructor
        {
            _repository = repository;
            _premioRepository = premioRepository;
            _residenteRepository = residenteRepository; // NUEVO

            AddCommand = new Command(async () => await AddAsync());
            LoadCommand = new Command(async () => await LoadAsync());
            ChangeEstadoCommand = new Command<int>(async (id) => await CambiarEstado(id));
            BuscarResidenteCommand = new Command(async () => await BuscarResidenteAsync()); // NUEVO

            _ = CargarPremioAsync();
            _ = LoadAsync();
        }

        public async Task CargarPremioAsync()
        {
            try
            {
                var lista = await _premioRepository.GetAllPremiosAsync();
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

        // NUEVO: Método para buscar Residente por DNI
        private async Task BuscarResidenteAsync()
        {
            if (string.IsNullOrWhiteSpace(DniResidente))
            {
                await AppShell.DisplayToastAsync("Ingrese un DNI válido.");
                return;
            }

            var residente = await _residenteRepository.ObtenerPorDniAsync(DniResidente);
            if (residente != null)
            {
                ResidenteEncontrado = residente;
                await AppShell.DisplayToastAsync($"Residente encontrado: {residente.NombreResidente}");
            }
            else
            {
                await AppShell.DisplayToastAsync("Residente no encontrado.");
            }
        }

        private async Task AddAsync()
        {
            var nuevo = new Models.Canje
            {
                FechaCanje = DateTime.Now,
                EstadoCanje = EstadoCanje,
                IdPremio = IdPremio,
                IdResidente = ResidenteEncontrado?.IdResidente ?? 0 // NUEVO: asigna IdResidente
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}