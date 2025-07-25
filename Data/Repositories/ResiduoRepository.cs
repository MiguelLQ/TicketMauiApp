﻿using MauiFirebase.Data.Interfaces;
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
    public async Task<bool> ChangeEstadoResiduoAsync(string id)
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
        var existeResiduo = await _database.Database!.Table<Residuo>()
                                 .FirstOrDefaultAsync(x => x.NombreResiduo!.ToLower() == residuo.NombreResiduo!.ToLower());
        if (existeResiduo != null)
        {
            Console.WriteLine("Existe residuo con ese nombre");
        }

        int resultado = await _database.Database!.InsertAsync(residuo);
        return residuo;
    }

    public async Task<List<Residuo>> GetAllResiduoAync()
    {
        var resultado = await _database.Database!.Table<Residuo>().ToListAsync();
        return resultado;
    }

    public Task<Residuo?> GetResiduoIdAsync(string id)
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

    public async Task MarcarComoSincronizadoAsync(string id)
    {
        var residuo = await GetResiduoIdAsync(id);
        if (residuo != null)
        {
            residuo.Sincronizado = true;
            await _database.Database!.UpdateAsync(residuo);
        }
    }

    public async Task<List<Residuo>> GetResiduosNoSincronizadosAsync()
    {
        return await _database.Database!.Table<Residuo>().Where(r => !r.Sincronizado).ToListAsync();
    }

    public async Task<bool> ExisteAsync(string id)
    {
        var residuo = await GetResiduoIdAsync(id);
        return residuo != null;
    }
}
