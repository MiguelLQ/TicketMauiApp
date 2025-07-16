using CommunityToolkit.Mvvm.ComponentModel;
using MauiFirebase.Models;
using MauiFirebase.Data.Interfaces;
using System.Threading.Tasks;

namespace MauiFirebase.PageModels.Residentes;

public partial class InicioCiudadanoPageModel : ObservableObject
{
    private readonly IResidenteRepository _residenteRepository;

    public InicioCiudadanoPageModel(IResidenteRepository residenteRepository)
    {
        _residenteRepository = residenteRepository;
    }

    [ObservableProperty]
    private int ticketsGanados;

    [ObservableProperty]
    private string nombreResidente;

    [ObservableProperty]
    private string apellidoResidente;

    public async Task CargarDatosUsuarioAsync()
    {
        var uid = Preferences.Get("FirebaseUid", string.Empty);
        var residente = await _residenteRepository.ObtenerPorUidFirebaseAsync(uid);

        if (residente != null)
        {
            TicketsGanados = residente.TicketsTotalesGanados;
            NombreResidente = residente.NombreResidente;
            ApellidoResidente = residente.ApellidoResidente;
        }
    }
}
