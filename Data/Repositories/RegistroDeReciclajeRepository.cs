using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
using SQLite;

namespace MauiFirebase.Data.Repositories;

public class RegistroDeReciclajeRepository : IRegistroDeReciclajeRepository
{
    private readonly AppDatabase _database;

    public RegistroDeReciclajeRepository(AppDatabase database)
    {
        _database = database;
        _ = _database.Database!.CreateTableAsync<RegistroDeReciclaje>(); // Asegura la tabla
    }

    public async Task GuardarAsync(RegistroDeReciclaje registro)
    {
        var existente = await ObtenerPorIdAsync(registro.IDRegistroDeReciclaje);
        if (existente == null)
        {
            registro.FechaRegistro = DateTime.Now;
            await _database.Database!.InsertAsync(registro);
        }
        else
        {
            await _database.Database!.UpdateAsync(registro);
        }
    }


    public async Task<List<RegistroDeReciclaje>> ObtenerTodosAsync()
    {
        return await _database.Database!.Table<RegistroDeReciclaje>().ToListAsync();
    }

    public async Task<RegistroDeReciclaje?> ObtenerPorIdAsync(string id)
    {
        return await _database.Database!.Table<RegistroDeReciclaje>()
                     .FirstOrDefaultAsync(r => r.IDRegistroDeReciclaje == id);
    }

    public async Task EliminarAsync(string id)
    {
        var registro = await ObtenerPorIdAsync(id);
        if (registro != null)
        {
            await _database.Database!.DeleteAsync(registro);
        }
    }

    public async Task<decimal> ObtenerTotalRecicladoKg()
    {
        var registros = await _database.Database!.Table<RegistroDeReciclaje>().ToListAsync();
        return registros.Sum(r => r.PesoKilogramo);
    }

    public async Task<List<ReciclajePorCategoria>> ObtenerTotalesPorCategoriaAsync()
    {
        var registros = await _database.Database!.Table<RegistroDeReciclaje>().ToListAsync();
        var residuos = await _database.Database!.Table<Residuo>().ToListAsync();

        var query = from r in registros
                    join res in residuos on r.IdResiduo equals res.IdResiduo
                    group r by res.NombreResiduo into g
                    select new ReciclajePorCategoria
                    {
                        Categoria = g.Key!,
                        TotalKg = g.Sum(x => x.PesoKilogramo)
                    };

        return query.ToList();
    }

    public async Task<List<RegistroDeReciclaje>> UltimosTresRegistros()
    {
        return await _database.Database!.Table<RegistroDeReciclaje>()
            .OrderByDescending(r => r.FechaRegistro).Take(3).ToListAsync();
    }

    public async Task MarcarComoSincronizadoAsync(string id)
    {
        var item = await _database.Database!.FindAsync<RegistroDeReciclaje>(id);
        if (item != null)
        {
            item.Sincronizado = true;
            await _database.Database.UpdateAsync(item);
        }
    }

    public async Task<List<RegistroDeReciclaje>> GetRegistrosNoSincronizadosAsync()
    {
        return await _database.Database!.Table<RegistroDeReciclaje>()
            .Where(c => !c.Sincronizado)
            .ToListAsync();
    }

    public async Task<bool> ExisteAsync(string id)
    {
        var lista = await ObtenerTodosAsync();
        return lista.Any(c => c.IDRegistroDeReciclaje.ToString() == id);
    }

}
