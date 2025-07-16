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
    private readonly ITicketRepository _localTicketRepository;
    private readonly IResiduoRepository _localResiduoRepository;
    private readonly ICategoriaResiduoRepository _localCategoriaResiduoRepository;
    private readonly IUbicacionVehiculo _localUbicacionVehiculoRepository;
    private readonly IRutaRepository _localRutaRepository;

    private readonly FirebaseUbicacionService _firebaseUbicacionService;
    private readonly FirebaseRegistroReciclajeService _firebaseReciclajeService;
    private readonly FirebaseResidenteService _firebaseResidenteService;
    private readonly FirebasePremioService _firebasePremioService;
    private readonly FirebaseCanjeService _firebaseCanjeService;
    private readonly FirebaseConvertidorService _firebaseConvertidorService;
    private readonly FirebaseVehiculoService _firebaseVehiculoService;
    private readonly FirebaseTicketService _firebaseTicketService;
    private readonly FirebaseResiduoService _firebaseResiduoService;
    private readonly FirebaseCategoriaResiduoService _firebaseCategoriaResiduoService;
    private readonly FirebaseRutaService _firebaseRutaService;
    private readonly FirebaseAuthService _authService;

    public SincronizacionFirebaseService(
        IRegistroDeReciclajeRepository localReciclajeRepository,
        IResidenteRepository localResidenteRepository,
        IPremioRepository localPremioRepository,
        ICanjeRepository localCanjeRepository,
        IConvertidorRepository localConvertidorRepository,
        IVehiculoRepository localVehiculoRepository,
        ITicketRepository localTicketRepository,
        IResiduoRepository localResiduoRepository,
        ICategoriaResiduoRepository localCategoriaResiduoRepository,
        IUbicacionVehiculo localUbicacionVehiculoRepository,
        IRutaRepository localRutaRepository,
        FirebaseRegistroReciclajeService firebaseReciclajeService,
        FirebaseResidenteService firebaseResidenteService,
        FirebasePremioService firebasePremioService,
        FirebaseCanjeService firebaseCanjeService,
        FirebaseConvertidorService firebaseConvertidorService,
        FirebaseVehiculoService firebaseVehiculoService,
        FirebaseTicketService firebaseTicketService,
        FirebaseResiduoService firebaseResiduoService,
        FirebaseUbicacionService firebaseUbicacionService,
        FirebaseCategoriaResiduoService firebaseCategoriaResiduoService,
        FirebaseRutaService firebaseRutaService,
        FirebaseAuthService authService)
    {
        _localReciclajeRepository = localReciclajeRepository;
        _localResidenteRepository = localResidenteRepository;
        _localPremioRepository = localPremioRepository;
        _localCanjeRepository = localCanjeRepository;
        _localConvertidorRepository = localConvertidorRepository;
        _localVehiculoRepository = localVehiculoRepository;
        _localTicketRepository = localTicketRepository;
        _localResiduoRepository = localResiduoRepository;
        _localCategoriaResiduoRepository = localCategoriaResiduoRepository;
        _localUbicacionVehiculoRepository = localUbicacionVehiculoRepository;
        _localRutaRepository = localRutaRepository;
        _firebaseReciclajeService = firebaseReciclajeService;
        _firebaseResidenteService = firebaseResidenteService;
        _firebasePremioService = firebasePremioService;
        _firebaseCanjeService = firebaseCanjeService;
        _firebaseConvertidorService = firebaseConvertidorService;
        _firebaseVehiculoService = firebaseVehiculoService;
        _firebaseTicketService = firebaseTicketService;
        _firebaseResiduoService = firebaseResiduoService;
        _firebaseUbicacionService = firebaseUbicacionService;
        _firebaseCategoriaResiduoService = firebaseCategoriaResiduoService;
        _firebaseRutaService = firebaseRutaService;
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

    /*==============================================================================================
     *                                        UbicacionVehiculo
     *==============================================================================================*/

    public async Task SincronizarUbicacionesAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var ubicacionesLocales = await _localUbicacionVehiculoRepository.ObtenerNoSincronizadosAsync();

        foreach (var ubicacion in ubicacionesLocales)
        {
            var exito = await _firebaseUbicacionService.GuardarUbicacionVehiculoFirestoreAsync(
                ubicacion, ubicacion.IdUbicacionVehiculo, idToken);

            if (exito)
            {
                await _localUbicacionVehiculoRepository.MarcarComoSincronizadoAsync(ubicacion.IdUbicacionVehiculo);
            }
        }
    }

    public async Task SincronizarUbicacionesDesdeFirebaseAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var ubicacionesRemotas = await _firebaseUbicacionService.ObtenerUbicacionesVehiculosDesdeFirestoreAsync(idToken);

        foreach (var remoto in ubicacionesRemotas)
        {
            var existe = await _localUbicacionVehiculoRepository.ExisteAsync(remoto.IdUbicacionVehiculo);
            if (!existe)
            {
                remoto.Sincronizado = true;
                await _localUbicacionVehiculoRepository.GuardarUbicacionAsync(remoto);
            }
            else
            {
                // Si ya existe, actualiza la ubicación local con la remota (última coordenada)
                remoto.Sincronizado = true;
                await _localUbicacionVehiculoRepository.GuardarUbicacionAsync(remoto);
            }
        }
    }

    /*==============================================================================================
    *                                    Convertidores
    *==============================================================================================*/

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

    public async Task SincronizarConvertidoresDesdeFirebaseAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var convertidoresRemotos = await _firebaseConvertidorService.ObtenerConvertidoresDesdeFirestoreAsync(idToken);

        foreach (var remoto in convertidoresRemotos)
        {
            var existe = await _localConvertidorRepository.ExisteAsync(remoto.IdConvertidor.ToString());
            if (!existe)
            {
                remoto.Sincronizado = true;
                await _localConvertidorRepository.CreateConvertidorAsync(remoto);
            }
        }
    }
    /*==============================================================================================
    *                                     Residentes
    *==============================================================================================*/

    public async Task SincronizarResidentesAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }
        var residentesNoSincronizados = await _localResidenteRepository.GetResidentesNoSincronizadosAsync();

        foreach (var residente in residentesNoSincronizados)
        {
            var exito = await _firebaseResidenteService.GuardarResidenteFirestoreAsync(
                residente, residente.IdResidente!, idToken);

            if (exito)
            {
                residente.Sincronizado = true;
                await _localResidenteRepository.UpdateResidenteAsync(residente);
            }
        }
    }


    public async Task SincronizarResidentesDesdeFirebaseAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var residenteRemotos = await _firebaseResidenteService.ObtenerResidentesDesdeFirestoreAsync(idToken);

        foreach (var remoto in residenteRemotos)
        {
            var existe = await _localResidenteRepository.ExisteAsync(remoto.IdResidente);
            if (!existe)
            {
                remoto.Sincronizado = true;
                await _localResidenteRepository.CreateResidenteAsync(remoto);
            }
        }
    }

    /*==============================================================================================
    *                                  Premio
    *==============================================================================================*/


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


    /*==============================================================================================
                                    * RegistrosReciclaje
    *==============================================================================================*/

    public async Task SincronizarRegistrosDeReciclajeAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }
        var registrosNoSincronizados = await _localReciclajeRepository.GetRegistrosNoSincronizadosAsync();
        foreach (var registro in registrosNoSincronizados)
        {
            var exito = await _firebaseReciclajeService.GuardarRegistroFirestoreAsync(
                registro, registro.IDRegistroDeReciclaje.ToString(), idToken);

            if (exito)
            {
                registro.Sincronizado = true;
                await _localReciclajeRepository.MarcarComoSincronizadoAsync(registro.IDRegistroDeReciclaje);
            }
        }
    }


    public async Task SincronizarRegistroReciclajeDesdeFirebaseAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var registrosRemotos = await _firebaseReciclajeService.ObtenerRegistrosDesdeFirestoreAsync(idToken);

        foreach (var remoto in registrosRemotos)
        {
            var existe = await _localReciclajeRepository.ExisteAsync(remoto.IDRegistroDeReciclaje);
            if (!existe)
            {
                remoto.Sincronizado = true;
                await _localReciclajeRepository.GuardarAsync(remoto);
            }
        }
    }
    /*==============================================================================================
                            * Canje
    *==============================================================================================*/
    public async Task SincronizarCanjeAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }
        var canjeNoSincronizados = await _localCanjeRepository.GetCanjesNoSincronizadosAsync();

        foreach (var canje in canjeNoSincronizados)
        {
            var exito = await _firebaseCanjeService.GuardarCanjeFirestoreAsync(
                canje, canje.IdCanje!, idToken);

            if (exito)
            {
                canje.Sincronizado = true;
                await _localCanjeRepository.UpdateCanjeAsync(canje);
            }
        }
    }
    public async Task SincronizarCanjesDesdeFirebaseAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
            return;

        var canjesRemotos = await _firebaseCanjeService.ObtenerCanjesDesdeFirestoreAsync(idToken);

        foreach (var remoto in canjesRemotos)
        {
            var existe = await _localCanjeRepository.ExisteAsync(remoto.IdCanje);
            if (!existe)
            {
                remoto.Sincronizado = true;
                await _localCanjeRepository.CreateCanjeAsync(remoto);
            }
            else
            {
                // Si quieres actualizar también:
                remoto.Sincronizado = true;
                await _localCanjeRepository.UpdateCanjeAsync(remoto);
            }
        }
    }


    /*==============================================================================================
                            * Vehiculo
    *==============================================================================================*/
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


    public async Task SincronizarVehiculoDesdeFirebaseAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var registrosRemotos = await _firebaseVehiculoService.ObtenerVehiculosDesdeFirestoreAsync(idToken);

        foreach (var remoto in registrosRemotos)
        {
            var existe = await _localVehiculoRepository.ExisteAsync(remoto.IdVehiculo);
            if (!existe)
            {
                remoto.Sincronizado = true;
                await _localVehiculoRepository.CreateVehiculoAsync(remoto);
            }
        }
    }

    /*==============================================================================================
                            * Ticket
    *==============================================================================================*/
    public async Task SincronizarTicketsAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null) return;
        var ticketsLocales = await _localTicketRepository.GetTicketsNoSincronizadosAsync();
        foreach (var ticket in ticketsLocales)
        {
            var exito = await _firebaseTicketService.GuardarTicketFirestoreAsync(ticket, ticket.IdTicket, idToken);
            if (exito)
            {
                await _localTicketRepository.MarcarComoSincronizadoAsync(ticket.IdTicket);
            }
        }
    }


    public async Task SincronizarTicketsDesdeFirebaseAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var ticketsRemotos = await _firebaseTicketService.ObtenerTicketsDesdeFirestoreAsync(idToken);

        foreach (var remoto in ticketsRemotos)
        {
            var existe = await _localTicketRepository.ExisteAsync(remoto.IdTicket);
            if (!existe)
            {
                remoto.Sincronizado = true;
                await _localTicketRepository.CreateTicketAsync(remoto);
            }
        }
    }

    /*==============================================================================================
                            * Residuos
    *==============================================================================================*/
    public async Task SincronizarResiduosAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null) return;
        var residuosLocales = await _localResiduoRepository.GetResiduosNoSincronizadosAsync();
        foreach (var residuo in residuosLocales)
        {
            var exito = await _firebaseResiduoService.GuardarResiduoFirestoreAsync(residuo, residuo.IdResiduo, idToken);
            if (exito)
            {
                await _localResiduoRepository.MarcarComoSincronizadoAsync(residuo.IdResiduo);
            }
        }
    }


    public async Task SincronizarResiduoDesdeFirebaseAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var residuoRemotos = await _firebaseResiduoService.ObtenerResiduosDesdeFirestoreAsync(idToken);

        foreach (var remoto in residuoRemotos)
        {
            var existe = await _localResiduoRepository.ExisteAsync(remoto.IdResiduo);
            if (!existe)
            {
                remoto.Sincronizado = true;
                await _localResiduoRepository.CreateResiduoAsync(remoto);
            }
        }
    }

    /*==============================================================================================
                            * CategoriaResiduo
     *==============================================================================================*/

    public async Task SincronizarCategoriasResiduoAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null) return;
        var categoriasLocales = await _localCategoriaResiduoRepository.GetCategoriasNoSincronizadasAsync();
        foreach (var categoria in categoriasLocales)
        {
            var exito = await _firebaseCategoriaResiduoService.GuardarCategoriaResiduoFirestoreAsync(categoria, categoria.IdCategoriaResiduo, idToken);
            if (exito)
            {
                await _localCategoriaResiduoRepository.MarcarComoSincronizadoAsync(categoria.IdCategoriaResiduo);
            }
        }
    }


    public async Task SincronizarCategoriaResiduoDesdeFirebaseAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }

        var categoriaResiduoRemotos = await _firebaseCategoriaResiduoService.ObtenerCategoriasResiduoDesdeFirestoreAsync(idToken);

        foreach (var remoto in categoriaResiduoRemotos)
        {
            var existe = await _localCategoriaResiduoRepository.ExisteAsync(remoto.IdCategoriaResiduo);
            if (!existe)
            {
                remoto.Sincronizado = true;
                await _localCategoriaResiduoRepository.CreateCategoriaResiduoAsync(remoto);
            }
        }
    }

    /*==============================================================================================
                            * Rutas
    *==============================================================================================*/
    public async Task SincronizarRutasAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }
        var rutasLocales = await _localRutaRepository.GetRutasNoSincronizadasAsync();
        foreach (var ruta in rutasLocales)
        {
            var exito = await _firebaseRutaService.GuardarRutaFirestoreAsync(ruta, ruta.IdRuta, idToken);
            if (exito)
            {
                await _localRutaRepository.MarcarComoSincronizadoAsync(ruta.IdRuta);
            }
        }
    }

    public async Task SincronizarRutasDesdeFirebaseAsync()
    {
        var idToken = await ObtenerTokenSiHayInternetAsync();
        if (idToken == null)
        {
            return;
        }
        var rutasRemotas = await _firebaseRutaService.ObtenerRutasDesdeFirestoreAsync(idToken);
        foreach (var remoto in rutasRemotas)
        {
            var existe = await _localRutaRepository.ExisteAsync(remoto.IdRuta);
            if (!existe)
            {
                remoto.Sincronizado = true;
                await _localRutaRepository.CreateRutaAsync(remoto);
            }
        }
    }
    // Si alguna vez necesitas sincronizar todo manualmente
    public async Task SincronizarTodoAsync()
    {
        // Subida
        await SincronizarResidentesAsync();
        await SincronizarPremiosAsync();
        await SincronizarConvertidoresAsync();
        await SincronizarRegistrosDeReciclajeAsync();
        await SincronizarCanjeAsync();
        await SincronizarVehiculosAsync();
        await SincronizarTicketsAsync();
        await SincronizarResiduosAsync();
        await SincronizarCategoriasResiduoAsync();
        await SincronizarRutasAsync();

        // Descarga desde Firebase (para mantener local actualizado)
        await SincronizarResidentesDesdeFirebaseAsync();
        await SincronizarRegistroReciclajeDesdeFirebaseAsync();
        await SincronizarConvertidoresDesdeFirebaseAsync();
        await SincronizarTicketsDesdeFirebaseAsync();
        await SincronizarResiduoDesdeFirebaseAsync();
        await SincronizarCategoriaResiduoDesdeFirebaseAsync();
        await SincronizarVehiculoDesdeFirebaseAsync();
        await SincronizarRutasDesdeFirebaseAsync();
    }
}
