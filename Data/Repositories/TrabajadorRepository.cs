using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;


namespace MauiFirebase.Data.Repositories;
public class TrabajadorRepository : ITrabajadorRepository
{
    private readonly AppDatabase _database;
    public TrabajadorRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<bool> ChangeEstadoTrabajadorAsync(int id)
    {
        Trabajador trabajador = await _database.Database!.Table<Trabajador>()
            .Where(t => t.IdTrabajador == id)
            .FirstOrDefaultAsync();
        if (trabajador == null)
            return false;
        trabajador.EstadoTrabajador = !trabajador.EstadoTrabajador;
        int result = await _database.Database.UpdateAsync(trabajador);
        return result > 0;
    }

    public async Task<Trabajador> CreateTrabajadorAsync(Trabajador trabajador)
    {
        int resultado = await _database.Database!.InsertAsync(trabajador);
        return trabajador;
    }

    public async Task<List<Trabajador>> GetAllTrabajadorAsync()
    {
        var resultado = await _database.Database!.Table<Trabajador>().ToListAsync();
        return resultado;
    }

    public Task<Trabajador?> GetTrabajadorByIdAsync(int id)
    {
        var resultado = _database.Database!.Table<Trabajador>()
            .Where(t => t.IdTrabajador == id)
            .FirstOrDefaultAsync();
        return resultado!;
    }

    public async Task<int> UpdateTrabajadorAsync(Trabajador trabajador)
    {
        var resultado = await _database.Database!.UpdateAsync(trabajador);
        return resultado;
    }
}
