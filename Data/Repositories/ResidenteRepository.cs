using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories;

public class ResidenteRepository : IResidenteRepository
{
    private readonly AppDatabase _database;  
    public ResidenteRepository(AppDatabase database)
    {
        _database = database;
        // Asegúrate de que la tabla Residente exista.
        // Simplemente llama al método y espera, sin el '_ ='
        _database.Database!.CreateTableAsync<Residente>().Wait(); // ¡Cambio aquí!
    }     

    public async Task<Residente> CreateResidenteAsync(Residente residente)
    {
        await _database.Database!.InsertAsync(residente);
        return residente;
    }

    public async Task<List<Residente>> GetAllResidentesAsync()
    {
        return await _database.Database!.Table<Residente>().ToListAsync();
    }

    public async Task<Residente?> GetResidenteByIdAsync(string id)
    {
        return await _database.Database!.Table<Residente>()
                                        .Where(r => r.IdResidente == id) 
                                        .FirstOrDefaultAsync();
    }

    public async Task<int> UpdateResidenteAsync(Residente residente)
    {
        residente.Sincronizado = false;
        return await _database.Database!.UpdateAsync(residente);
    }

    public async Task<bool> ChangeEstadoResidenteAsync(string id) 
    {
        Residente? residente = await _database.Database!.Table<Residente>()
                                               .Where(r => r.IdResidente == id)
                                            .FirstOrDefaultAsync();
        if (residente == null)
        {
            return false;
        }

        residente.EstadoResidente = !residente.EstadoResidente; 

        int result = await _database.Database!.UpdateAsync(residente);
        return result > 0;
    }
    public async Task<List<Residente>> SearchResidentesByNameOrApellidoAsync(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {

            return await GetAllResidentesAsync();
        }

        string lowerSearchText = searchText.ToLower();

        return await _database.Database!.Table<Residente>()
                                        .Where(r => (r.NombreResidente != null && r.NombreResidente.ToLower().Contains(lowerSearchText)) ||
                                                    (r.ApellidoResidente != null && r.ApellidoResidente.ToLower().Contains(lowerSearchText)))
                                        .ToListAsync();
    }

    public async Task<Residente?> GetResidenteByDniAsync(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
        {
            return null;
        }

        return await _database.Database!.Table<Residente>()
                                        .Where(r => r.DniResidente == dni) 
                                        .FirstOrDefaultAsync();
    }

    public async Task<Residente?> ObtenerPorDniAsync(string dniResidente)
    {
        if (string.IsNullOrWhiteSpace(dniResidente))
        {
            return null;
        }

        return await _database.Database!.Table<Residente>()
                                        .Where(r => r.DniResidente == dniResidente)
                                        .FirstOrDefaultAsync();
    }
    public async Task GuardarAsync(Residente residenteEncontrado)
    {
        residenteEncontrado.Sincronizado = false; 
        await _database.Database!.UpdateAsync(residenteEncontrado);
    }
    public async Task<int> TotalResidentes()
    {
        return await _database.Database!.Table<Residente>().CountAsync();
    }
    public async Task<Residente?> ObtenerPorIdAsync(string idResidente)
    {
        return await _database.Database!.Table<Residente>()
            .Where(r => r.IdResidente == idResidente)
            .FirstOrDefaultAsync();
    }
    public async Task<bool> ExisteAsync(string id)
    {
        var lista = await GetAllResidentesAsync();
        return lista.Any(r => r.IdResidente.ToString() == id);
    }

    public async Task<List<Residente>> GetResidentesNoSincronizadosAsync()
    {
        return await _database.Database!.Table<Residente>()
            .Where(c => !c.Sincronizado)
            .ToListAsync();
    }

    public async Task MarcarComoSincronizadoAsync(string id)
    {
        var item = await _database.Database!.FindAsync<Residente>(id);
        if (item != null)
        {
            item.Sincronizado = true;
            await _database.Database.UpdateAsync(item);
        }
    }
    public async Task<Residente?> ObtenerPorUidFirebaseAsync(string uid)
    {
        if (string.IsNullOrEmpty(uid))
            return null;

        return await _database.Database!.Table<Residente>()
            .Where(r => r.UidFirebase == uid)
            .FirstOrDefaultAsync();
    }
    public async Task<int> ObtenerTicketsGanadosPorUidAsync(string uidFirebase)
    {
        if (string.IsNullOrEmpty(uidFirebase))
            return 0;

        var residente = await _database.Database!.Table<Residente>()
            .Where(r => r.UidFirebase == uidFirebase)
            .FirstOrDefaultAsync();

        return residente?.TicketsTotalesGanados ?? 0;
    }

}