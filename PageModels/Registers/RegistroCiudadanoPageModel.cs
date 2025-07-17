using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;

namespace MauiFirebase.PageModels.Registers
{
    public partial class RegistroCiudadanoPageModel : ObservableObject
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

        // 🔹 Propiedades del formulario
        [ObservableProperty] private string? nombre;
        [ObservableProperty] private string? apellido;
        [ObservableProperty] private string? dni;
        [ObservableProperty] private string? correo;
        [ObservableProperty] private string? direccion;
        [ObservableProperty] private bool estadoResidente = true;

        // 🔹 Control de la vista
        [ObservableProperty] private bool mostrarFormulario = true;
        [ObservableProperty] private string? qrBase64;

        // 🔹 Datos del usuario mostrado
        [ObservableProperty] private string? nombreResidenteLocal;
        [ObservableProperty] private string? apellidoResidenteLocal;
        [ObservableProperty] private string? correoResidenteLocal;
        [ObservableProperty] private string? direccionResidenteLocal;
        [ObservableProperty] private string? dniResidenteLocal;

        // 🔹 Se ejecuta al cargar la vista
        public async Task InicializarAsync()
        {
            var uid = Preferences.Get("FirebaseUserId", null);
            var idToken = Preferences.Get("FirebaseToken", null);

            if (string.IsNullOrEmpty(uid))
                return;

            // 1. Intentar obtener desde Preferences (si ya está guardado)
            var idResidente = Preferences.Get("IdResidenteFirestore", null);
            Residente? residente = null;

            if (!string.IsNullOrEmpty(idResidente))
            {
                residente = await _residenteRepository.ObtenerPorIdAsync(idResidente);

                // ⚠️ Validar que el UID del residente local coincida con el UID actual
                if (residente != null && residente.UidFirebase != uid)
                {
                    residente = null; // Ignorar datos si no pertenecen al usuario autenticado
                }
            }

            if (residente != null)
            {
                MostrarDatosResidente(residente);
                MostrarFormulario = false;
                return; // Ya está todo listo
            }

            // 2. Si hay internet, consultar Firestore por el UID actual
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

            // 🔹 Si no se encontró en local ni en Firestore → mostrar el formulario
            MostrarFormulario = true;
        }


        [RelayCommand]
        private async Task GuardarCiudadanoAsync()
        {
            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Apellido) ||
                string.IsNullOrWhiteSpace(Correo) ||
                string.IsNullOrWhiteSpace(Direccion))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Por favor complete todos los campos.", "OK");
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
                UidFirebase = uid
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
