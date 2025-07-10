using CommunityToolkit.Mvvm.ComponentModel;
using MauiFirebase.Data.Interfaces;

namespace MauiFirebase.PageModels.CamScaners;

public partial class CamScanerPageModel : ObservableObject
{
    private readonly IResidenteRepository _residenteRepository;

    public CamScanerPageModel(IResidenteRepository residenteRepository)
    {
        _residenteRepository = residenteRepository;
    }

    [ObservableProperty]
    private string? codigoDetectado;

    public async Task<bool> ValidarDniExistenteAsync(string dni)
    {
        var residente = await _residenteRepository.ObtenerPorDniAsync(dni);
        return residente != null;
    }
}
