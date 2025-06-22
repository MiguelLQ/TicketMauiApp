using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiFirebase.PageModels.Premio
{
    public class PremioPageModel : INotifyPropertyChanged
    {
        private readonly IPremioRepository _premioRepository;

        public ObservableCollection<Models.Premio> Premios { get; set; } = new();

        private string _nombrePremio;
        public string NombrePremio
        {
            get => _nombrePremio;
            set { _nombrePremio = value; OnPropertyChanged(); }
        }

        private string _descripcionPremio;
        public string DescripcionPremio
        {
            get => _descripcionPremio;
            set { _descripcionPremio = value; OnPropertyChanged(); }
        }

        private int _puntosRequeridos;
        public int PuntosRequeridos
        {
            get => _puntosRequeridos;
            set { _puntosRequeridos = value; OnPropertyChanged(); }
        }

        private bool _estadoPremio = true;
        public bool EstadoPremio
        {
            get => _estadoPremio;
            set { _estadoPremio = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoadPremiosCommand { get; }
        public ICommand AddPremioCommand { get; }
        public ICommand ChangeEstadoCommand { get; }

        public PremioPageModel(IPremioRepository premioRepository)
        {
            _premioRepository = premioRepository;

            LoadPremiosCommand = new Command(async () => await LoadPremiosAsync());
            AddPremioCommand = new Command(async () => await AddPremioAsync());
            ChangeEstadoCommand = new Command<int>(async id => await ChangeEstadoAsync(id));

            _ = LoadPremiosAsync(); // auto-load premios
        }

        private async Task LoadPremiosAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Premios.Clear();
                var premios = await _premioRepository.GetAllPremiosAsync();
                foreach (var p in premios)
                {
                    Debug.WriteLine($"Premio: {p.NombrePremio}, {p.DescripcionPremio}, {p.PuntosRequeridos}");
                    Premios.Add(p);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }


        private async Task AddPremioAsync()
        {
            if (string.IsNullOrWhiteSpace(NombrePremio) || string.IsNullOrWhiteSpace(DescripcionPremio))
                return;

            var newPremio = new Models.Premio
            {
                NombrePremio = NombrePremio,
                DescripcionPremio = DescripcionPremio,
                PuntosRequeridos = PuntosRequeridos,
                EstadoPremio = EstadoPremio
            };

            await _premioRepository.CreatePremioAsync(newPremio);
            await LoadPremiosAsync();

            NombrePremio = string.Empty;
            DescripcionPremio = string.Empty;
            PuntosRequeridos = 0;
            EstadoPremio = true;
        }

        private async Task ChangeEstadoAsync(int id)
        {
            await _premioRepository.ChangePremioStatusAsync(id);
            await LoadPremiosAsync();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
