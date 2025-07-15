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

    public async Task<bool> ChangeEstadoRutaAsync(string id)
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

    public Task<Ruta?> GetRutaIdAsync(string id)
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

    public async Task MarcarComoSincronizadoAsync(string id)
    {
        var item = await _database.Database!.FindAsync<Ruta>(id);
        if (item != null)
        {
            item.Sincronizado = true;
            await _database.Database.UpdateAsync(item);
        }
    }

    public async Task<List<Ruta>> GetRutasNoSincronizadasAsync()
    {
        return await _database.Database!.Table<Ruta>()
            .Where(r => !r.Sincronizado)
            .ToListAsync();
    }

    public async Task<bool> ExisteAsync(string id)
    {
        var lista = await GetAllRutaAsync();
        return lista.Any(r => r.IdRuta == id);
    }
}
