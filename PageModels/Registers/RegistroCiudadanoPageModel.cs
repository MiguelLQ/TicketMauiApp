using System.Buffers.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using MauiFirebase.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace MauiFirebase.PageModels.Registers
{
    public partial class RegistroCiudadanoPageModel : ObservableObject
    {
        private readonly FirebaseAuthService _authService;
        private readonly IResidenteRepository _residenteRepository;
        private readonly FirebaseCiudadanoService _firebaseCiudadanoService = new FirebaseCiudadanoService();

        public RegistroCiudadanoPageModel(
            FirebaseAuthService authService,
            IResidenteRepository residenteRepository)
        {
            _authService = authService;
            _residenteRepository = residenteRepository;
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
        // para mostrar los datos del usuario
        [ObservableProperty] private string? nombreResidenteLocal;
        [ObservableProperty] private string? apellidoResidenteLocal;
        [ObservableProperty] private string? correoResidenteLocal;
        [ObservableProperty] private string? direccionResidenteLocal;

        // 🔹 Se ejecuta al cargar la vista
        public async Task InicializarAsync()
        {
            var uid = Preferences.Get("FirebaseUserId", null);
            var idToken = Preferences.Get("FirebaseToken", null);

            if (string.IsNullOrEmpty(uid) || string.IsNullOrEmpty(idToken))
                return;

            var yaExiste = await _firebaseCiudadanoService.ResidenteExisteEnFirestoreAsync(uid, idToken);
            var residente = await _residenteRepository.ObtenerPorUidAsync(uid);

            if (yaExiste)
            {
                NombreResidenteLocal = residente.NombreResidente;
                ApellidoResidenteLocal = residente.ApellidoResidente;
                CorreoResidenteLocal = residente.CorreoResidente;
                DireccionResidenteLocal = residente.DireccionResidente;
                MostrarFormulario = false;

                // Opcional: cargar más datos desde Firestore si los necesitas
                var dataQr = $"UID:{uid}\nDNI:{residente.DniResidente}";
                QrBase64 = GenerarQrComoBase64(dataQr);
            }
            else
            {
                MostrarFormulario = true;
            }
        }

        // 🔹 Comando para guardar los datos del ciudadano
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
                Uid = uid,
                NombreResidente = Nombre,
                ApellidoResidente = Apellido,
                DniResidente = Dni,
                CorreoResidente = Correo,
                DireccionResidente = Direccion,
                EstadoResidente = EstadoResidente,
                FechaRegistroResidente = DateTime.Now,
                TicketsTotalesGanados = 0
            };

            // 🔸 Guardar local
            await _residenteRepository.CreateResidenteAsync(nuevoResidente);

            // 🔸 Guardar en Firestore
            var idToken = await _authService.ObtenerIdTokenSeguroAsync();
            var exito = await _firebaseCiudadanoService.GuardarEnFirestoreAsync(nuevoResidente, uid, idToken);

            if (exito)
            {
                MostrarFormulario = false;
                QrBase64 = GenerarQrComoBase64($"UID:{uid}");

                await Application.Current.MainPage.DisplayAlert("Registro exitoso", "Tu información ha sido guardada correctamente en Firestore.", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Error al guardar en Firestore.", "OK");
            }
        }

        // 🔹 Método auxiliar para generar un QR base64
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
