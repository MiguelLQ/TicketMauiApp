using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces;
public interface IUbicacionVehiculo
{
    Task<UbicacionVehiculo?> ObtenerUbicacionAsync(int idVehiculo);
    Task GuardarUbicacionAsync(UbicacionVehiculo ubicacion);
    Task EliminarUbicacionAsync(int idVehiculo);
    Task<List<UbicacionVehiculo>> ObtenerTodasAsync();
}
