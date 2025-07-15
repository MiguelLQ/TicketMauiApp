using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
namespace MauiFirebase.Data.Repositories;

public class CanjeRepository : ICanjeRepository
{
    private readonly AppDatabase _database;

    public CanjeRepository(AppDatabase database)
    {
        _database = database;

        // Asegura que la tabla exista
        Task.Run(async () => await _database.Database!.CreateTableAsync<Canje>()).Wait();
    }

    public async Task<Canje> CreateCanjeAsync(Canje canje)
    {
        canje.FechaCanje = DateTime.Now;
        await _database.Database!.InsertAsync(canje);
        return canje;
    }

    public async Task<List<Canje>> GetAllCanjeAync()
    {
        return await _database.Database!.Table<Canje>().ToListAsync();
    }

    public async Task<Canje?> GetCanjeIdAsync(string id)
    {
        return await _database.Database!.Table<Canje>()
            .FirstOrDefaultAsync(c => c.IdCanje == id);
    }

    public async Task<int> UpdateCanjeAsync(Canje canje)
    {
        return await _database.Database!.UpdateAsync(canje);
    }

    public async Task<bool> ChangeEstadoCanjeAsync(string id)
    {
        var canje = await _database.Database!.Table<Canje>()
            .FirstOrDefaultAsync(c => c.IdCanje == id);
        if (canje == null)
            return false;

        canje.EstadoCanje = !canje.EstadoCanje;
        int result = await _database.Database.UpdateAsync(canje);
        return result > 0;
    }
    public async Task<Canje?> ObtenerPorUidAsync(string idCanje)
    {
        return await _database.Database!.Table<Canje>()
            .Where(r => r.IdCanje == idCanje)
            .FirstOrDefaultAsync();
    }
    public async Task<bool> ExisteAsync(string id)
    {
        var lista = await GetAllCanjeAync();
        return lista.Any(r => r.IdCanje.ToString() == id);
    }
    public async Task<List<Canje>> GetCanjesNoSincronizadosAsync()
    {
        return await _database.Database!.Table<Canje>()
            .Where(c => !c.Sincronizado)
            .ToListAsync();
    }
    public async Task MarcarComoSincronizadoAsync(string id)
    {
        var item = await _database.Database!.FindAsync<Canje>(id);
        if (item != null)
        {
            item.Sincronizado = true;
            await _database.Database.UpdateAsync(item);
        }
    }
}
