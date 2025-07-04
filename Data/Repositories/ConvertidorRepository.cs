﻿using MauiFirebase.Data.Interfaces;
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
    public async Task<bool> ChangeEstadoConvertidorAsync(int id)
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

    public  Task<Convertidor?> GetConvertidorIdAsync(int id)
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
}
