using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MauiFirebase.Data.Repositories;
using MauiFirebase.Helpers.Interface;
using MauiFirebase.Pages.usuario;

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
        public Command EditarUsuarioCommand { get; }
        public Command<Models.Usuario> IrEditarUsuarioCommand { get; }


        public UsuarioPageModel(IAlertaHelper alertaHelper)
        {
            _usuarioRepository = new UsuarioRepository();
            GuardarUsuarioCommand = new Command(async () => await GuardarUsuarioAsync());
            IrAgregarUsuarioCommand = new Command(async () => await IrAgregarUsuarioAsync());
            IrEditarUsuarioCommand = new Command<Models.Usuario>(async (usuario) => await IrEditarUsuarioAsync(usuario));
            EditarUsuarioCommand = new Command(async () => await EditarUsuarioAsync());
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
                UsuarioNuevo.FotoLocal = "userlogo.png"; // imagen local embebida
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
        // Este método se llama desde el botón de edición
        public async Task<bool> EditarUsuarioAsync()
        {
            return await EditarUsuarioAsync(UsuarioNuevo);
        }
        //Este método hace la lógica de edición en local y en Firestore
        public async Task<bool> EditarUsuarioAsync(Models.Usuario usuarioEditado)
        {
            try
            {
                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    await _alertaHelper.ShowErrorAsync("No tienes conexión a internet.");
                    return false;
                }

                var usuarioExistente = await _usuarioRepository.ObtenerUsuarioPorUidAsync(usuarioEditado.Uid!);
                if (usuarioExistente == null)
                {
                    await _alertaHelper.ShowErrorAsync("Usuario no encontrado.");
                    return false;
                }

                // 🔒 Solo se editan estos campos por parte del admin
                usuarioExistente.Rol = usuarioEditado.Rol;
                usuarioExistente.Estado = usuarioEditado.Estado;

                var actualizado = await _usuarioRepository.EditarUsuarioLocalAsync(usuarioExistente);

                if (actualizado)
                {
                    // 🔁 También actualizar en Firestore
                    var token = Preferences.Get("FirebaseToken", string.Empty);
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        var firebaseService = new FirebaseUsuarioService();
                        await firebaseService.EditarUsuarioEnFirestoreAsync(usuarioExistente, token);
                    }

                    await _alertaHelper.ShowSuccessAsync("Usuario actualizado correctamente.");
                    await Shell.Current.GoToAsync("..");
                    return true;
                }

                await _alertaHelper.ShowErrorAsync("No se pudo actualizar el usuario.");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al editar usuario: " + ex.Message);
                await _alertaHelper.ShowErrorAsync("Ocurrió un error al editar el usuario.");
                return false;
            }
        }


        private async Task IrAgregarUsuarioAsync()
        {
            await Shell.Current.GoToAsync("///usuarios/ListarUsuarioPage");
        }
        private async Task IrEditarUsuarioAsync(Models.Usuario usuario)
        {
            UsuarioNuevo = usuario;
            await Shell.Current.GoToAsync("///usuarios/editar");
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
