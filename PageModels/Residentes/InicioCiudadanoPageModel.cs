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

    [ObservableProperty] private int ticketsGanados;
    [ObservableProperty] private string nombreResidente = "Ciudadano";
    [ObservableProperty] private string apellidoResidente = "";
    [ObservableProperty] private string ticketsGanadosTexto = "Cargando...";
    [ObservableProperty]
    private bool isBusy;

    public string NombreCompleto => $"{NombreResidente} {ApellidoResidente}";

    partial void OnNombreResidenteChanged(string value) =>
        OnPropertyChanged(nameof(NombreCompleto));

    partial void OnApellidoResidenteChanged(string value) =>
        OnPropertyChanged(nameof(NombreCompleto));

    public async Task CargarDatosUsuarioAsync()
    {
        try
        {
            var uid = Preferences.Get("FirebaseUserId", string.Empty);

            if (string.IsNullOrWhiteSpace(uid))
            {
                TicketsGanadosTexto = "UID no encontrado";
                return;
            }

            var residenteLocal = await _residenteRepository.ObtenerPorUidFirebaseAsync(uid);

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var token = await _authService.ObtenerIdTokenSeguroAsync();
                var residentesFirebase = await _firebaseService.ObtenerResidentesDesdeFirestoreAsync(token);

                var residenteFirestore = residentesFirebase?.FirstOrDefault(r => r.UidFirebase == uid);

                if (residenteFirestore != null)
                {
                    residenteFirestore.Sincronizado = true;

                    if (residenteLocal == null)
                        await _residenteRepository.CreateResidenteAsync(residenteFirestore);
                    else
                        await _residenteRepository.UpdateResidenteAsync(residenteFirestore);

                    residenteLocal = residenteFirestore;
                }
            }

            if (residenteLocal != null)
            {
                TicketsGanados = residenteLocal.TicketsTotalesGanados;
                TicketsGanadosTexto = residenteLocal.TicketsTotalesGanados.ToString();
                NombreResidente = residenteLocal.NombreResidente;
                ApellidoResidente = residenteLocal.ApellidoResidente;
            }
            else
            {
                TicketsGanadosTexto = "Datos no disponibles";
            }
        }
        catch (Exception ex)
        {
            // Opcional: usa AppCenter o similar para loguear errores
            TicketsGanadosTexto = "Error cargando datos";
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
}

