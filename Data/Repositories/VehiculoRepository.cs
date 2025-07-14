using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories;
public class VehiculoRepository : IVehiculoRepository
{
    private readonly AppDatabase _database;
    public VehiculoRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<bool> ChangeEstadoVehiculoAsync(string id)
    {
        Vehiculo vehiculo = await _database.Database!.Table<Vehiculo>()
            .Where(v => v.IdVehiculo == id)
            .FirstOrDefaultAsync();
        if (vehiculo == null)
            return false;
        vehiculo.EstadoVehiculo = !vehiculo.EstadoVehiculo;
        int result = await _database.Database.UpdateAsync(vehiculo);
        return result > 0;
    }

    public async Task<Vehiculo> CreateVehiculoAsync(Vehiculo vehiculo)
    {
        int resultado = await _database.Database!.InsertAsync(vehiculo);
        return vehiculo;
    }

    public async Task<List<Vehiculo>> GetAllVehiculoAsync()
    {
        var resultado = await _database.Database!.Table<Vehiculo>().ToListAsync();
        return resultado;
    }

    public Task<Vehiculo?> GetVehiculoByIdAsync(string id)
    {
        var resultado = _database.Database!.Table<Vehiculo>()
            .Where(v => v.IdVehiculo == id)
            .FirstOrDefaultAsync();
        return resultado!;
    }

    public async Task<int> UpdateVehiculoAsync(Vehiculo vehiculo)
    {
        var resultado = await _database.Database!.UpdateAsync(vehiculo);
        return resultado;
    }
    public async Task<Vehiculo?> GetVehiculoPorPlacaAsync(string placa)
    {
        var resultado = await _database.Database!.Table<Vehiculo>()
            .Where(v => v.PlacaVehiculo == placa)
            .FirstOrDefaultAsync();
        return resultado;
    }
}
