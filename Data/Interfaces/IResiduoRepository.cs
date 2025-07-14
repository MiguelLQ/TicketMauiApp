using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces;
public interface IResiduoRepository
{
    Task<List<Residuo>> GetAllResiduoAync();
    Task<Residuo> CreateResiduoAsync(Residuo residuo);
    Task<Residuo?> GetResiduoIdAsync(string id);
    Task<int> UpdateResiduoAsync(Residuo residuo);
    Task<bool> ChangeEstadoResiduoAsync(string id);
    Task MarcarComoSincronizadoAsync(string id);
    Task<List<Residuo>> GetResiduosNoSincronizadosAsync();
    Task<bool> ExisteAsync(string id);
}
