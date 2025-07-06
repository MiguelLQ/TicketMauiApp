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

    public async Task<Canje?> GetCanjeIdAsync(int id)
    {
        return await _database.Database!.Table<Canje>()
            .FirstOrDefaultAsync(c => c.IdCanje == id);
    }

    public async Task<int> UpdateCanjeAsync(Canje canje)
    {
        return await _database.Database!.UpdateAsync(canje);
    }

    public async Task<bool> ChangeEstadoCanjeAsync(int id)
    {
        var canje = await _database.Database!.Table<Canje>()
            .FirstOrDefaultAsync(c => c.IdCanje == id);
        if (canje == null)
            return false;

        canje.EstadoCanje = !canje.EstadoCanje;
        int result = await _database.Database.UpdateAsync(canje);
        return result > 0;
    }
}
