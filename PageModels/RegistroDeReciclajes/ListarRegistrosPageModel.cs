using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;

namespace MauiFirebase.PageModels.RegistroDeReciclajes
{
    public class ListarRegistrosPageModel : INotifyPropertyChanged
    {
        private readonly IRegistroDeReciclajeRepository _registroRepository;

        public ObservableCollection<RegistroDeReciclaje> Registros { get; set; } = new();

        public ICommand EliminarRegistroCommand { get; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ListarRegistrosPageModel(IRegistroDeReciclajeRepository registroRepository)
        {
            _registroRepository = registroRepository;
            EliminarRegistroCommand = new Command<int>(async (id) => await EliminarRegistroAsync(id));
            _ = LoadRegistrosAsync();
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

        private async Task EliminarRegistroAsync(int id)
        {
            await _registroRepository.EliminarAsync(id);
            await LoadRegistrosAsync();
            await AppShell.DisplayToastAsync("Registro eliminado.");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
