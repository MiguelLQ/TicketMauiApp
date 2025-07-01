using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Data.Repositories;

namespace MauiFirebase.PageModels.Usuarios
{
    public class UsuarioPageModel: INotifyPropertyChanged
    {
        private readonly UsuarioRepository _usuarioRepository;

        public ObservableCollection<Models.Usuario> Usuarios { get; } = new();

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public UsuarioPageModel()
        {
            _usuarioRepository = new UsuarioRepository();
            _ = CargarUsuariosAsync(); // No espera, pero ejecuta
        }

        public async Task CargarUsuariosAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;

                var lista = await _usuarioRepository.GetUsuariosAsync();
                Usuarios.Clear();

                foreach (var usuario in lista)
                    Usuarios.Add(usuario);
            }
            catch (Exception ex)
            {
                // Podrías mostrar un toast/snackbar aquí
                Console.WriteLine($"Error cargando usuarios: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
