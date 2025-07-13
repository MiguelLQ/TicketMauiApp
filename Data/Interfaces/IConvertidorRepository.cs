using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces;
public interface IConvertidorRepository
{
    Task<List<Convertidor>> GetAllConvertidorAync();
    Task<Convertidor> CreateConvertidorAsync(Convertidor convertidor);
    Task<Convertidor?> GetConvertidorIdAsync(int id);
    Task<int> UpdateConvertidorAsync(Convertidor convertidor);
    Task<bool> ChangeEstadoConvertidorAsync(int id);
    Task MarcarComoSincronizadoAsync(int id);
    Task<List<Convertidor>> GetConvertidoresNoSincronizadosAsync();
}
