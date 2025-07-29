using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System.ComponentModel.DataAnnotations;

namespace MauiFirebase.PageModels.Registers
{
    public partial class RegistroCiudadanoPageModel : ObservableValidator
    {
        private readonly FirebaseAuthService _authService;
        private readonly IResidenteRepository _residenteRepository;
        private readonly FirebaseCiudadanoService _firebaseCiudadanoService = new FirebaseCiudadanoService();
        private readonly SincronizacionFirebaseService _sincronizador;

        public RegistroCiudadanoPageModel(
            FirebaseAuthService authService,
            IResidenteRepository residenteRepository,
            SincronizacionFirebaseService sincronizador)
        {
            _authService = authService;
            _residenteRepository = residenteRepository;
            _sincronizador = sincronizador;
        }

        // =================== PROPIEDADES CON VALIDACIÓN ===================

        [ObservableProperty]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 20 caracteres.")]
        private string nombre = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "El apellido debe tener entre 3 y 20 caracteres.")]
        private string apellido = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "El DNI es obligatorio.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener 8 dígitos.")]
        private string dni = string.Empty;

        [ObservableProperty]
        [EmailAddress(ErrorMessage = "El correo no es válido.")]
        private string correo = string.Empty;

        [ObservableProperty]
        private string direccion = string.Empty;

        [ObservableProperty]
        private bool estadoResidente = true;

        // =================== ERRORES DNI DUPLICADO ===================

        public string? DniDuplicadoError { get; private set; }
        public bool HasDniDuplicadoError => !string.IsNullOrWhiteSpace(DniDuplicadoError);

        // =================== CONTROL DE VISTA ===================

        [ObservableProperty]
        private bool mostrarFormulario = true;

        [ObservableProperty]
        private string? qrBase64;

        // =================== DATOS DEL USUARIO MOSTRADO ===================

        [ObservableProperty]
        private string? nombreResidenteLocal;

        [ObservableProperty]
        private string? apellidoResidenteLocal;

        [ObservableProperty]
        private string? correoResidenteLocal;

        [ObservableProperty]
        private string? direccionResidenteLocal;

        [ObservableProperty]
        private string? dniResidenteLocal;

        // =================== MÉTODO DE INICIALIZACIÓN ===================

        public async Task InicializarAsync()
        {
            var uid = Preferences.Get("FirebaseUserId", null);
            var idToken = Preferences.Get("FirebaseToken", null);

            if (string.IsNullOrEmpty(uid))
                return;

            var idResidente = Preferences.Get("IdResidenteFirestore", null);
            Residente? residente = null;

            if (!string.IsNullOrEmpty(idResidente))
            {
                residente = await _residenteRepository.ObtenerPorIdAsync(idResidente);

                if (residente != null && residente.UidFirebase != uid)
                {
                    residente = null;
                }
            }

            if (residente != null)
            {
                MostrarDatosResidente(residente);
                MostrarFormulario = false;
                return;
            }

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet && !string.IsNullOrEmpty(idToken))
            {
                var residenteFirestore = await _firebaseCiudadanoService.ObtenerResidentePorUidFirebaseAsync(uid, idToken);

                if (residenteFirestore != null)
                {
                    var local = await _residenteRepository.ObtenerPorIdAsync(residenteFirestore.IdResidente);

                    if (local == null)
                        await _residenteRepository.CreateResidenteAsync(residenteFirestore);
                    else
                        await _residenteRepository.UpdateResidenteAsync(residenteFirestore);

                    Preferences.Set("IdResidenteFirestore", residenteFirestore.IdResidente);
                    MostrarDatosResidente(residenteFirestore);
                    MostrarFormulario = false;
                    return;
                }
            }

            MostrarFormulario = true;
            if (string.IsNullOrWhiteSpace(Correo))
            {
                var correoUsuario = _authService.GetUserEmail();
                if (!string.IsNullOrWhiteSpace(correoUsuario))
                    Correo = correoUsuario;
            }
        }

        // =================== COMANDO GUARDAR CIUDADANO ===================

        [RelayCommand]
        public async Task GuardarCiudadanoAsync()
        {
            ValidateAllProperties();

            if (HasErrors || HasDniDuplicadoError)
            {
                var errores = string.Join("\n", GetErrors().Select(e => e.ErrorMessage));
                if (HasDniDuplicadoError)
                    errores += $"\n{DniDuplicadoError}";
                await Application.Current.MainPage.DisplayAlert("Errores", errores, "OK");
                return;
            }

            var uid = Preferences.Get("FirebaseUserId", null);
            if (string.IsNullOrEmpty(uid))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo obtener el usuario autenticado.", "OK");
                return;
            }

            var nuevoResidente = new Residente
            {
                NombreResidente = Nombre,
                ApellidoResidente = Apellido,
                DniResidente = Dni,
                CorreoResidente = Correo,
                DireccionResidente = Direccion,
                EstadoResidente = EstadoResidente,
                FechaRegistroResidente = DateTime.Now,
                TicketsTotalesGanados = 0,
                UidFirebase = uid,
                Sincronizado = false
            };

            await _residenteRepository.CreateResidenteAsync(nuevoResidente);
            Preferences.Set("IdResidenteFirestore", nuevoResidente.IdResidente);

            var idToken = await _authService.ObtenerIdTokenSeguroAsync();
            var exito = await _firebaseCiudadanoService.GuardarEnFirestoreAsync(nuevoResidente, idToken);

            if (exito)
            {
                MostrarFormulario = false;
                QrBase64 = GenerarQrComoBase64($"ID:{nuevoResidente.IdResidente}\nDNI:{nuevoResidente.DniResidente}");
                await Application.Current.MainPage.DisplayAlert("Registro exitoso", "Tu información ha sido guardada correctamente.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Error al guardar en Firestore.", "OK");
            }
        }

        // =================== VALIDACIÓN EN TIEMPO REAL ===================

        partial void OnNombreChanged(string value)
        {
            ValidateProperty(value, nameof(Nombre));
            OnPropertyChanged(nameof(NombreError));
            OnPropertyChanged(nameof(PuedeGuardar));
        }

        partial void OnApellidoChanged(string value)
        {
            ValidateProperty(value, nameof(Apellido));
            OnPropertyChanged(nameof(ApellidoError));
            OnPropertyChanged(nameof(PuedeGuardar));
        }

        partial void OnDniChanged(string value)
        {
            ValidateProperty(value, nameof(Dni));
            _ = VerificarDniDuplicadoAsync(value);
            OnPropertyChanged(nameof(DniError));
            OnPropertyChanged(nameof(DniDuplicadoError));
            OnPropertyChanged(nameof(HasDniDuplicadoError));
            OnPropertyChanged(nameof(PuedeGuardar));
        }

        partial void OnCorreoChanged(string value)
        {
            ValidateProperty(value, nameof(Correo));
            OnPropertyChanged(nameof(CorreoError));
            OnPropertyChanged(nameof(PuedeGuardar));
        }

        partial void OnDireccionChanged(string value)
        {
            ValidateProperty(value, nameof(Direccion));
            OnPropertyChanged(nameof(DireccionError));
            OnPropertyChanged(nameof(PuedeGuardar));
        }

        private async Task VerificarDniDuplicadoAsync(string dni)
        {
            DniDuplicadoError = null;

            if (!string.IsNullOrWhiteSpace(dni) && dni.Length == 8)
            {
                var existente = await _residenteRepository.GetResidenteByDniAsync(dni);
                if (existente != null)
                {
                    DniDuplicadoError = "Ya existe un residente con este DNI.";
                }
            }

            OnPropertyChanged(nameof(DniDuplicadoError));
            OnPropertyChanged(nameof(HasDniDuplicadoError));
            OnPropertyChanged(nameof(PuedeGuardar));
        }

        // =================== ERRORES PARA XAML ===================

        public string? NombreError => GetErrors(nameof(Nombre)).FirstOrDefault()?.ErrorMessage;
        public string? ApellidoError => GetErrors(nameof(Apellido)).FirstOrDefault()?.ErrorMessage;
        public string? DniError => GetErrors(nameof(Dni)).FirstOrDefault()?.ErrorMessage;
        public string? CorreoError => GetErrors(nameof(Correo)).FirstOrDefault()?.ErrorMessage;
        public string? DireccionError => GetErrors(nameof(Direccion)).FirstOrDefault()?.ErrorMessage;

        // =================== PROPIEDAD PARA CONTROLAR EL BOTÓN GUARDAR ===================

        public bool PuedeGuardar => !HasErrors && !HasDniDuplicadoError
            && !string.IsNullOrWhiteSpace(Nombre)
            && !string.IsNullOrWhiteSpace(Apellido)
            && !string.IsNullOrWhiteSpace(Dni);

        // =================== MÉTODOS AUXILIARES ===================

        private void MostrarDatosResidente(Residente residente)
        {
            NombreResidenteLocal = residente.NombreResidente;
            ApellidoResidenteLocal = residente.ApellidoResidente;
            CorreoResidenteLocal = residente.CorreoResidente;
            DireccionResidenteLocal = residente.DireccionResidente;
            DniResidenteLocal = residente.DniResidente;

            QrBase64 = GenerarQrComoBase64($"ID:{residente.IdResidente}\nDNI:{residente.DniResidente}");
        }

        private string GenerarQrComoBase64(string texto)
        {
            using var qrGenerator = new QRCoder.QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(texto, QRCoder.QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new QRCoder.PngByteQRCode(qrCodeData);
            var qrBytes = qrCode.GetGraphic(20);
            return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
        }
    }
}
