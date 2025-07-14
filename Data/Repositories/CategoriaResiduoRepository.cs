using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
namespace MauiFirebase.Data.Repositories;
public class CategoriaResiduoRepository : ICategoriaResiduoRepository
{
    private readonly AppDatabase _database;

    public CategoriaResiduoRepository(AppDatabase db)
    {
        _database = db;
        _ = _database.Database!.CreateTableAsync<CategoriaResiduo>(); // asegúrate de crear la tabla  
    }

    public async Task<CategoriaResiduo> CreateCategoriaResiduoAsync(CategoriaResiduo categoriaResiduo)
    {
        await _database.Database!.InsertAsync(categoriaResiduo);
        return categoriaResiduo;
    }

    public async Task<List<CategoriaResiduo>> GetAllCategoriaResiduoAsync()
    {
        return await _database.Database!.Table<CategoriaResiduo>().ToListAsync();
    }

    public async Task<CategoriaResiduo?> GetCategoriaResiduoIdAsync(string id)
    {
        return await _database.Database!.Table<CategoriaResiduo>().FirstOrDefaultAsync(t => t.IdCategoriaResiduo == id);
    }

    public async Task<int> UpdateCategoriaResiduoAsync(CategoriaResiduo categoriaResiduo)
    {
        return await _database.Database!.UpdateAsync(categoriaResiduo);
    }

    public async Task<bool> ChangeEstadoCategoriaResiduoAsync(string id)
    {
        var categoria = await GetCategoriaResiduoIdAsync(id);
        if (categoria == null) return false;

        categoria.EstadoCategoriaResiduo = !categoria.EstadoCategoriaResiduo;
        await _database.Database!.UpdateAsync(categoria);
        return true;
    }

    // Método adicional para obtener categorías por ID de ticket (relación)  
    public async Task<List<CategoriaResiduo>> GetByTicketIdAsync(string ticketId)
    {
        return await _database.Database!.Table<CategoriaResiduo>()
                        .Where(c => c.IdTicket == ticketId)
                        .ToListAsync();
    }

    public async Task MarcarComoSincronizadoAsync(string id)
    {
        var categoria = await GetCategoriaResiduoIdAsync(id);
        if (categoria != null)
        {
            categoria.Sincronizado = true;
            await _database.Database!.UpdateAsync(categoria);
        }
    }

    public async Task<List<CategoriaResiduo>> GetCategoriasNoSincronizadasAsync()
    {
        return await _database.Database!.Table<CategoriaResiduo>().Where(c => !c.Sincronizado).ToListAsync();
    }

    public async Task<bool> ExisteAsync(string id)
    {
        var categoria = await GetCategoriaResiduoIdAsync(id);
        return categoria != null;
    }
}
