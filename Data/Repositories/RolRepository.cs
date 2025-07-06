
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories;
public class RolRepository
{
    private readonly AppDatabase _database;

    public RolRepository(AppDatabase database)
    {
        _database = database;
    }

    public async Task<List<Rol>> GetAllRolesAsync()
    {
        return await _database.Database!.Table<Rol>().ToListAsync();
    }

    public async Task SeedRolesAsync()
    {
        var roles = await _database.Database!.Table<Rol>().ToListAsync();
        if (!roles.Any())
        {
            var lista = new List<Rol>
            {
                new() { NombreRol = "Administrador" },
                new() { NombreRol = "Recolector" },
                new() { NombreRol = "Conductor" },
                new() { NombreRol = "Residente" }// FIREBASE
            };

            await _database.Database!.InsertAllAsync(lista);
        }
    }
}
