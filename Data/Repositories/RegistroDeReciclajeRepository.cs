using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        if (registro.IDRegistroDeReciclaje != 0)
        {
            await _database.Database!.UpdateAsync(registro);
        }
        else
        {
            registro.FechaRegistro = DateTime.Now; // Registrar fecha actual
            await _database.Database!.InsertAsync(registro);
        }
    }

    public async Task<List<RegistroDeReciclaje>> ObtenerTodosAsync()
    {
        return await _database.Database!.Table<RegistroDeReciclaje>().ToListAsync();
    }

    public async Task<RegistroDeReciclaje?> ObtenerPorIdAsync(int id)
    {
        return await _database.Database!.Table<RegistroDeReciclaje>()
                     .FirstOrDefaultAsync(r => r.IDRegistroDeReciclaje == id);
    }

    public async Task EliminarAsync(int id)
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
}
