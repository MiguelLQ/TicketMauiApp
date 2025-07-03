using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Data.Repositories;
using MauiFirebase.Helpers.Interface;

namespace MauiFirebase.PageModels.Usuarios
{
    public class UsuarioPageModel: INotifyPropertyChanged
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly IAlertaHelper _alertaHelper;

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
        private Models.Usuario _usuarioNuevo = new();
        public Models.Usuario UsuarioNuevo
        {
            get => _usuarioNuevo;
            set
            {
                _usuarioNuevo = value;
                OnPropertyChanged();
            }
        }
        public Command GuardarUsuarioCommand { get; }
        public Command IrAgregarUsuarioCommand { get; }


        public UsuarioPageModel(IAlertaHelper alertaHelper)
        {
            _usuarioRepository = new UsuarioRepository();
            GuardarUsuarioCommand = new Command(async () => await GuardarUsuarioAsync());
            IrAgregarUsuarioCommand = new Command(async () => await IrAgregarUsuarioAsync());

            _alertaHelper = alertaHelper;

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
        public async Task<bool> GuardarUsuarioAsync()
        {
            try
            {
                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    await _alertaHelper.ShowErrorAsync("No tienes conexión a internet.");
                    return false;
                }
                if (string.IsNullOrWhiteSpace(UsuarioNuevo.Nombre) ||
                    string.IsNullOrWhiteSpace(UsuarioNuevo.Correo) ||
                    string.IsNullOrWhiteSpace(UsuarioNuevo.Rol))
                {
                    await _alertaHelper.ShowErrorAsync("Completa todos los campos.");
                    return false;
                }

                var agregado = await _usuarioRepository.AgregarUsuarioAsync(UsuarioNuevo);
                if (agregado)
                {
                    Usuarios.Add(UsuarioNuevo); // Ya viene con el UID asignado en el repositorio
                    UsuarioNuevo = new();       // Limpiar formulario
                    await _alertaHelper.ShowSuccessAsync("Usuario creado correctamente.");
                    await Shell.Current.GoToAsync("..");
                    return true;
                }
                else
                {
                    await _alertaHelper.ShowErrorAsync("No se pudo crear el usuario.");
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al guardar usuario: " + ex.Message);
                await _alertaHelper.ShowErrorAsync("Error al guardar usuario.");
                return false;
            }
        }

        private async Task IrAgregarUsuarioAsync()
        {
            await Shell.Current.GoToAsync("///usuarios/ListarUsuarioPage");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
