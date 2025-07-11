using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        public List<string> RolesDisponibles { get; } = new()
        {
            "Administrador",
            "Recolector",
            "Conductor",
        };

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
                // 1. Verificar conexión
                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    await _alertaHelper.ShowErrorAsync("No tienes conexión a internet.");
                    return false;
                }

                // 2. Validar campos obligatorios
                if (string.IsNullOrWhiteSpace(UsuarioNuevo.Nombre) ||
                    string.IsNullOrWhiteSpace(UsuarioNuevo.Apellido) ||
                    string.IsNullOrWhiteSpace(UsuarioNuevo.Correo) ||
                    string.IsNullOrWhiteSpace(UsuarioNuevo.Telefono) ||
                    string.IsNullOrWhiteSpace(UsuarioNuevo.Rol) ||
                    string.IsNullOrWhiteSpace(UsuarioNuevo.Contraseña))
                {
                    await _alertaHelper.ShowErrorAsync("Completa todos los campos obligatorios.");
                    return false;
                }

                // 3. Establecer valores por defecto
                UsuarioNuevo.Foto = "userlogo.png"; // imagen local embebida
                UsuarioNuevo.Estado = true;

                // 4. Guardar en Firebase (excepto la foto) y en SQLite (con foto local)
                var agregado = await _usuarioRepository.AgregarUsuarioAsync(UsuarioNuevo);

                if (agregado)
                {
                    Usuarios.Add(UsuarioNuevo);     // Agregar a la lista local
                    UsuarioNuevo = new();           // Limpiar el formulario

                    await _alertaHelper.ShowSuccessAsync("Usuario creado correctamente.");
                    await Shell.Current.GoToAsync(".."); // volver atrás
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
