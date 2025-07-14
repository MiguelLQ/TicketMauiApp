using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces;
public interface IConvertidorRepository
{
    Task<List<Convertidor>> GetAllConvertidorAync();
    Task<Convertidor> CreateConvertidorAsync(Convertidor convertidor);
    Task<Convertidor?> GetConvertidorIdAsync(string id);
    Task<int> UpdateConvertidorAsync(Convertidor convertidor);
    Task<bool> ChangeEstadoConvertidorAsync(string id);
    Task MarcarComoSincronizadoAsync(string id);
    Task<List<Convertidor>> GetConvertidoresNoSincronizadosAsync();
    Task<bool> ExisteAsync(string id);
}
