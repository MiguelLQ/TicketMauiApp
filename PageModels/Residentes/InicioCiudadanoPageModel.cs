using CommunityToolkit.Mvvm.ComponentModel;
using MauiFirebase.Models;
using MauiFirebase.Data.Interfaces;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Residentes;

public partial class InicioCiudadanoPageModel : ObservableObject
{
    private readonly IResidenteRepository _residenteRepository;
    private readonly FirebaseAuthService _authService;
    private readonly FirebaseResidenteService _firebaseService;

    public InicioCiudadanoPageModel(
        IResidenteRepository residenteRepository,
        FirebaseAuthService authService,
        FirebaseResidenteService firebaseService)
    {
        _residenteRepository = residenteRepository;
        _authService = authService;
        _firebaseService = firebaseService;
    }

    // 🔹 Datos individuales
    [ObservableProperty] private int ticketsGanados;
    [ObservableProperty] private string nombreResidente;
    [ObservableProperty] private string apellidoResidente;
    [ObservableProperty] private string ticketsGanadosTexto;

    // 🔹 Nombre completo calculado (no editable)
    public string NombreCompleto => $"{NombreResidente} {ApellidoResidente}";

    // 🔹 Notificar cuando cambia nombre o apellido para actualizar NombreCompleto
    partial void OnNombreResidenteChanged(string value) =>
        OnPropertyChanged(nameof(NombreCompleto));

    partial void OnApellidoResidenteChanged(string value) =>
        OnPropertyChanged(nameof(NombreCompleto));

    public async Task CargarDatosUsuarioAsync()
    {
        var uid = Preferences.Get("FirebaseUserId", string.Empty);
        if (string.IsNullOrEmpty(uid)) return;

        // 🔹 Obtener desde SQLite
        var residenteLocal = await _residenteRepository.ObtenerPorUidFirebaseAsync(uid);

        // 🔹 Si hay internet, intenta actualizar desde Firestore
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            var token = await _authService.ObtenerIdTokenSeguroAsync();
            var residentesFirebase = await _firebaseService.ObtenerResidentesDesdeFirestoreAsync(token);
            var residenteFirestore = residentesFirebase.FirstOrDefault(r => r.UidFirebase == uid);

            if (residenteFirestore != null)
            {
                // 🔄 Sincronizar si hay cambios
                if (residenteLocal == null ||
                    residenteFirestore.TicketsTotalesGanados != residenteLocal.TicketsTotalesGanados ||
                    residenteFirestore.NombreResidente != residenteLocal.NombreResidente ||
                    residenteFirestore.ApellidoResidente != residenteLocal.ApellidoResidente)
                {
                    residenteFirestore.Sincronizado = true;

                    if (residenteLocal == null)
                        await _residenteRepository.CreateResidenteAsync(residenteFirestore);
                    else
                        await _residenteRepository.UpdateResidenteAsync(residenteFirestore);

                    residenteLocal = residenteFirestore;
                }
            }
        }

        // 🔹 Mostrar lo que se tenga local
        if (residenteLocal != null)
        {
            TicketsGanados = residenteLocal.TicketsTotalesGanados;
            TicketsGanadosTexto = residenteLocal.TicketsTotalesGanados.ToString();
            NombreResidente = residenteLocal.NombreResidente;
            ApellidoResidente = residenteLocal.ApellidoResidente;
        }
        else
        {
            TicketsGanadosTexto = "No disponible";
            NombreResidente = "Ciudadano";
            ApellidoResidente = "";
        }
    }
}
