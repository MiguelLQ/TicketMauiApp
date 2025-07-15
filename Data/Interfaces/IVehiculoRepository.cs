using MauiFirebase.Models;
namespace MauiFirebase.Data.Interfaces;
public interface IVehiculoRepository
{
    Task<List<Vehiculo>> GetAllVehiculoAsync();
    Task<Vehiculo> CreateVehiculoAsync(Vehiculo vehiculo);
    Task<Vehiculo?> GetVehiculoByIdAsync(string id);
    Task<int> UpdateVehiculoAsync(Vehiculo vehiculo);
    Task<bool> ChangeEstadoVehiculoAsync(string id);
    Task<Vehiculo?> GetVehiculoPorPlacaAsync(string placa);
    Task<Vehiculo?> GetByUsuarioUidAsync(string uid);

}
