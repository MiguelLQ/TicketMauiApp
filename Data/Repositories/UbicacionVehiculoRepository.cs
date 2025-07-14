using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories;
public class UbicacionVehiculoRepository : IUbicacionVehiculo
{
    private readonly AppDatabase _database;
    public UbicacionVehiculoRepository(AppDatabase database)
    {
        _database = database;
    }
    public async Task EliminarUbicacionAsync(string idVehiculo)
    {
        var ubicacion = await _database.Database!.Table<UbicacionVehiculo>()
            .FirstOrDefaultAsync(u => u.IdVehiculo == idVehiculo);
        if (ubicacion != null)
        {
            await _database.Database.DeleteAsync(ubicacion);
        }
    }

    public async Task GuardarUbicacionAsync(UbicacionVehiculo ubicacion)
    {
        await _database.Database!.InsertOrReplaceAsync(ubicacion);
    }

    public Task<List<UbicacionVehiculo>> ObtenerTodasAsync()
    {
        return _database.Database!.Table<UbicacionVehiculo>().ToListAsync();

    }

    public async Task<UbicacionVehiculo?> ObtenerUbicacionAsync(string idVehiculo)
    {
        return await _database.Database!.Table<UbicacionVehiculo>()
            .FirstOrDefaultAsync(u => u.IdVehiculo == idVehiculo);
    }
}
