using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
using SQLite;

namespace MauiFirebase.Data.Repositories
{
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

        public async Task<CategoriaResiduo?> GetCategoriaResiduoIdAsync(int id)
        {
            return await _database.Database!.Table<CategoriaResiduo>().FirstOrDefaultAsync(t => t.IdCategoriaResiduo == id);
        }

        public async Task<int> UpdateCategoriaResiduoAsync(CategoriaResiduo categoriaResiduo)
        {
            return await _database.Database!.UpdateAsync(categoriaResiduo);
        }

        public async Task<bool> ChangeEstadoCategoriaResiduoAsync(int id)
        {
            var categoria = await GetCategoriaResiduoIdAsync(id);
            if (categoria == null) return false;

            categoria.EstadoCategoriaResiduo = !categoria.EstadoCategoriaResiduo;
            await _database.Database!.UpdateAsync(categoria);
            return true;
        }

        // Método adicional para obtener categorías por ID de ticket (relación)  
        public async Task<List<CategoriaResiduo>> GetByTicketIdAsync(int ticketId)
        {
            return await _database.Database!.Table<CategoriaResiduo>()
                            .Where(c => c.IdTicket == ticketId)
                            .ToListAsync();
        }
    }
}
