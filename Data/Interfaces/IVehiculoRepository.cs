using MauiFirebase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiFirebase.Data.Interfaces;
public interface IVehiculoRepository
{
    Task<List<Vehiculo>> GetAllVehiculoAsync();
    Task<Vehiculo> CreateVehiculoAsync(Vehiculo vehiculo);
    Task<Vehiculo?> GetVehiculoByIdAsync(int id);
    Task<int> UpdateVehiculoAsync(Vehiculo vehiculo);
    Task<bool> ChangeEstadoVehiculoAsync(int id);
    Task<Vehiculo?> GetVehiculoPorPlacaAsync(string placa);

}
