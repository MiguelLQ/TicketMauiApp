using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories;
public class ResiduoRepository : IResiduoRepository
{
    private readonly AppDatabase _database;
    public ResiduoRepository(AppDatabase database)
    {
        _database = database;
    }
    public async Task<bool> ChangeEstadoResiduoAsync(int id)
    {
        Residuo residuo = await _database.Database!.Table<Residuo>()
                              .Where(r => r.IdResiduo == id)
                              .FirstOrDefaultAsync();
        if (residuo == null)
        {
            return false;
        }
        residuo.EstadoResiduo = !residuo.EstadoResiduo;
        int result = await _database.Database.UpdateAsync(residuo);
        return result > 0;

    }

    public async Task<Residuo> CreateResiduoAsync(Residuo residuo)
    {
        var resultado = await _database.Database!.InsertAsync(residuo);
        return residuo;
    }

    public async Task<List<Residuo>> GetAllResiduoAync()
    {
        var resultado = await _database.Database!.Table<Residuo>().ToListAsync();
        return resultado;
    }

    public Task<Residuo?> GetResiduoIdAsync(int id)
    {
        var resultado = _database.Database!.Table<Residuo>()
                              .Where(r => r.IdResiduo == id)
                              .FirstOrDefaultAsync();
        return resultado!;
    }

    public async Task<int> UpdateResiduoAsync(Residuo residuo)
    {
        var resultado = await _database.Database!.UpdateAsync(residuo);
        return resultado;
    }
}
