using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiFirebase.Data.Interfaces;

namespace MauiFirebase.PageModels.CategoriaResiduos
{
    public class CategoriaResiduoPageModel : INotifyPropertyChanged
    {
        private readonly ICategoriaResiduoRepository _repository;

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
        public ICommand ChangeEstadoCommand { get; }

        public CategoriaResiduoPageModel(ICategoriaResiduoRepository repository)
        {
            _repository = repository;
            AddCommand = new Command(async () => await AddAsync());
            LoadCommand = new Command(async () => await LoadAsync());
            ChangeEstadoCommand = new Command<int>(async (id) => await CambiarEstado(id));
        }

        private async Task LoadAsync()
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
