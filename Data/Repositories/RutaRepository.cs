using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories;

public class RutaRepository : IRutaRepository
{
    private readonly AppDatabase _database;

    public RutaRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<bool> ChangeEstadoRutaAsync(int id)
    {
        Ruta ruta = await _database.Database!.Table<Ruta>()
                          .Where(r => r.IdRuta == id)
                          .FirstOrDefaultAsync();

        if (ruta == null)
        {
            return false;
        }

        ruta.EstadoRuta = !ruta.EstadoRuta;
        int result = await _database.Database.UpdateAsync(ruta);
        return result > 0;
    }

    public async Task<Ruta> CreateRutaAsync(Ruta ruta)
    {
        ruta.FechaRegistroRuta = DateTime.Now;
        int resultado = await _database.Database!.InsertAsync(ruta);
        return ruta;
    }

    public async Task<List<Ruta>> GetAllRutaAsync()
    {
        var resultado = await _database.Database!.Table<Ruta>().ToListAsync();
        return resultado;
    }

    public Task<Ruta?> GetRutaIdAsync(int id)
    {
        var resultado = _database.Database!.Table<Ruta>()
                          .Where(r => r.IdRuta == id)
                          .FirstOrDefaultAsync();
        return resultado!;
    }

    public async Task<int> UpdateRutaAsync(Ruta ruta)
    {
        var resultado = await _database.Database!.UpdateAsync(ruta);
        return resultado;
    }
}
