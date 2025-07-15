using MauiFirebase.Models;
namespace MauiFirebase.Data.Interfaces;

public interface IResidenteRepository
{
    Task<List<Residente>> GetAllResidentesAsync();
    Task<Residente> CreateResidenteAsync(Residente residente);
    Task<Residente?> GetResidenteByIdAsync(string id);
    Task<int> UpdateResidenteAsync(Residente residente);
    Task<bool> ChangeEstadoResidenteAsync(string id); 
    Task<List<Residente>> SearchResidentesByNameOrApellidoAsync(string searchText);
    Task<Residente?> GetResidenteByDniAsync(string dni);
    Task<Residente?> ObtenerPorDniAsync(string dniResidente);
    Task GuardarAsync(Residente residenteEncontrado);
    Task<int> TotalResidentes();
    //  para trear datos locales segun su uid
    Task<Residente?> ObtenerPorIdAsync(string uid);
    Task MarcarComoSincronizadoAsync(string id);
    Task<List<Residente>> GetResidentesNoSincronizadosAsync();
    Task<bool> ExisteAsync(string id);
    Task<Residente?> ObtenerPorUidFirebaseAsync(string uid);

}
