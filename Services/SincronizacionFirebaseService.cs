using MauiFirebase.Data.Interfaces;

namespace MauiFirebase.Services;

public class SincronizacionFirebaseService
{
    /*========================================================================
     * Repositorios locales para acceder a los datos almacenados en SQLite
     * =====================================================================*/
    private readonly IRegistroDeReciclajeRepository _localReciclajeRepository;
    private readonly IResidenteRepository _localResidenteRepository;
    private readonly IPremioRepository _localPremioRepository;
    private readonly ICanjeRepository _localCanjeRepository;
    private readonly IConvertidorRepository _localConvertidorRepository;
    private readonly IVehiculoRepository _localVehiculoRepository;

    private readonly FirebaseRegistroReciclajeService _firebaseReciclajeService;
    private readonly FirebaseResidenteService _firebaseResidenteService;
    private readonly FirebasePremioService _firebasePremioService;
    private readonly FirebaseCanjeService _firebaseCanjeService;
    private readonly FirebaseConvertidorService _firebaseConvertidorService;
    private readonly FirebaseVehiculoService _firebaseVehiculoService;
    private readonly FirebaseAuthService _authService;

    public SincronizacionFirebaseService(
        IRegistroDeReciclajeRepository localReciclajeRepository,
        IResidenteRepository localResidenteRepository,
        IPremioRepository localPremioRepository,
        ICanjeRepository localCanjeRepository,
        IConvertidorRepository localConvertidorRepository,
        IVehiculoRepository localVehiculoRepository,
        FirebaseRegistroReciclajeService firebaseReciclajeService,
        FirebaseResidenteService firebaseResidenteService,
        FirebasePremioService firebasePremioService,
        FirebaseCanjeService firebaseCanjeService,
        FirebaseConvertidorService firebaseConvertidorService,
        FirebaseVehiculoService firebaseVehiculoService,
        FirebaseAuthService authService)
    {
        _localReciclajeRepository = localReciclajeRepository;
        _localResidenteRepository = localResidenteRepository;
        _localPremioRepository = localPremioRepository;
        _localCanjeRepository = localCanjeRepository;
        _localConvertidorRepository = localConvertidorRepository;
        _localVehiculoRepository = localVehiculoRepository;
        _firebaseReciclajeService = firebaseReciclajeService;
        _firebaseResidenteService = firebaseResidenteService;
        _firebasePremioService = firebasePremioService;
        _firebaseCanjeService = firebaseCanjeService;
        _firebaseConvertidorService = firebaseConvertidorService;
        _firebaseVehiculoService = firebaseVehiculoService;
        _authService = authService;
    }

    private async Task<string?> ObtenerTokenSiHayInternetAsync()
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
        {
            return null;
        }
        return await _authService.ObtenerIdTokenSeguroAsync();
    }

    public async Task SincronizarConvertidoresAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var convertidoresLocales = await _localConvertidorRepository.GetAllConvertidorAync();
        foreach (var convertidor in convertidoresLocales.Where(c => !c.Sincronizado))
        {
            var exito = await _firebaseConvertidorService.GuardarConvertidorFirestoreAsync(convertidor, convertidor.IdConvertidor.ToString(), idToken);
            if (exito)
            {
                await _localConvertidorRepository.MarcarComoSincronizadoAsync(convertidor.IdConvertidor);
            }
        }
    }

    public async Task SincronizarResidentesAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var residentesLocales = await _localResidenteRepository.GetAllResidentesAsync();
        foreach (var residente in residentesLocales)
        {
            await _firebaseResidenteService.GuardarResidenteFirestoreAsync(residente, residente.IdResidente.ToString(), idToken);
        }
    }

    public async Task SincronizarPremiosAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var premiosLocales = await _localPremioRepository.GetAllPremiosAsync();
        foreach (var premio in premiosLocales)
        {
            await _firebasePremioService.GuardarPremioFirestoreAsync(premio, premio.IdPremio.ToString(), idToken);
        }
    }

    public async Task SincronizarRegistrosDeReciclajeAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var registrosLocales = await _localReciclajeRepository.ObtenerTodosAsync();
        foreach (var registro in registrosLocales)
        {
            await _firebaseReciclajeService.GuardarRegistroFirestoreAsync(registro, registro.IDRegistroDeReciclaje.ToString(), idToken);
        }
    }

    public async Task SincronizarCanjesAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null) 
        { 
            return; 
        }

        var canjesLocales = await _localCanjeRepository.GetAllCanjeAync();
        foreach (var canje in canjesLocales)
        {
            await _firebaseCanjeService.GuardarCanjeFirestoreAsync(canje, canje.IdCanje.ToString(), idToken);
        }
    }

    public async Task SincronizarVehiculosAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var vehiculosLocales = await _localVehiculoRepository.GetAllVehiculoAsync();
        foreach (var vehiculo in vehiculosLocales)
        {
            await _firebaseVehiculoService.GuardarVehiculoFirestoreAsync(vehiculo, vehiculo.IdVehiculo.ToString(), idToken);
        }
    }

    // Si alguna vez necesitas sincronizar todo manualmente
    public async Task SincronizarTodoAsync()
    {
        await SincronizarResidentesAsync();
        await SincronizarPremiosAsync();
        await SincronizarConvertidoresAsync();
        await SincronizarRegistrosDeReciclajeAsync();
        await SincronizarCanjesAsync();
        await SincronizarVehiculosAsync();
    }
}
