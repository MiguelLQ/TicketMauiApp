using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces;
public interface IUbicacionVehiculo
{
    Task<UbicacionVehiculo?> ObtenerUbicacionAsync(string idVehiculo);
    Task GuardarUbicacionAsync(UbicacionVehiculo ubicacion);
    Task EliminarUbicacionAsync(string idVehiculo);
    Task<List<UbicacionVehiculo>> ObtenerTodasAsync();
    Task<List<UbicacionVehiculo>> ObtenerNoSincronizadosAsync();
    Task MarcarComoSincronizadoAsync(string id);
    Task<bool> ExisteAsync(string id);
}
