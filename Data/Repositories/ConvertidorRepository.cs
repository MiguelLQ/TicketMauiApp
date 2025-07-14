using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
namespace MauiFirebase.Data.Repositories;
public class ConvertidorRepository : IConvertidorRepository
{
    private readonly AppDatabase _database;
    public ConvertidorRepository(AppDatabase database)
    {
        _database= database;
    }
    public async Task<bool> ChangeEstadoConvertidorAsync(string id)
    {
        Convertidor convertidor = await _database.Database!.Table<Convertidor>()
                              .Where(r => r.IdConvertidor == id)
                              .FirstOrDefaultAsync();
        if (convertidor == null)
        {
            return false;
        }
        convertidor.EstadoConvertidor = !convertidor.EstadoConvertidor;
        int result = await _database.Database.UpdateAsync(convertidor);
        return result > 0;
    }

    public async Task<Convertidor> CreateConvertidorAsync(Convertidor convertidor)
    {
        int resultado = await _database.Database!.InsertAsync(convertidor);
        return convertidor;
    }

    public async Task<List<Convertidor>> GetAllConvertidorAync()
    {
        List<Convertidor> resultado = await _database.Database!.Table<Convertidor>().ToListAsync();
        return resultado;
    }

    public  Task<Convertidor?> GetConvertidorIdAsync(string id)
    {
        Task<Convertidor> resultado = _database.Database!.Table<Convertidor>()
                             .Where(r => r.IdConvertidor == id)
                             .FirstOrDefaultAsync();
        return resultado!;
    }

    public async Task<int> UpdateConvertidorAsync(Convertidor convertidor)
    {
        var resultado = await _database.Database!.UpdateAsync(convertidor);
        return resultado;
    }

    public async Task<List<Convertidor>> GetConvertidoresNoSincronizadosAsync()
    {
        return await _database.Database!.Table<Convertidor>()
            .Where(c => !c.Sincronizado)
            .ToListAsync();
    }

    public async Task MarcarComoSincronizadoAsync(string id)
    {
        var item = await _database.Database!.FindAsync<Convertidor>(id);
        if (item != null)
        {
            item.Sincronizado = true;
            await _database.Database.UpdateAsync(item);
        }
    }

    public async Task<bool> ExisteAsync(string id)
    {
        var lista = await GetAllConvertidorAync();
        return lista.Any(c => c.IdConvertidor.ToString() == id);
    }
}
