using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces;

public interface IRutaRepository
{
    Task<List<Ruta>> GetAllRutaAsync();
    Task<Ruta> CreateRutaAsync(Ruta ruta);
    Task<Ruta?> GetRutaIdAsync(string id);
    Task<int> UpdateRutaAsync(Ruta ruta);
    Task<bool> ChangeEstadoRutaAsync(string id);
}
