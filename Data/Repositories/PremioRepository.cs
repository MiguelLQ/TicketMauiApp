using System.Collections.Generic;
using System.Threading.Tasks;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories
{
    public class PremioRepository : IPremioRepository
    {
        private readonly AppDatabase _database;

        public PremioRepository(AppDatabase database)
        {
            _database = database;
            _ = _database.Database!.CreateTableAsync<Premio>(); // asegúrate de crear la tabla

        }

        public async Task<Premio> CreatePremioAsync(Premio premio)
        {
            await _database.Database!.InsertAsync(premio); // Inserta toda la entidad
            return premio;
        }


        public async Task<List<Premio>> GetAllPremiosAsync()
        {
            return await _database.Database!.Table<Premio>().ToListAsync();
        }

        public async Task<Premio?> GetPremioByIdAsync(int id)
        {
            return await _database.Database!.Table<Premio>().FirstOrDefaultAsync(t => t.IdPremio == id);
        }

        public async Task<int> UpdatePremioAsync(Premio premio)
        {
            return await _database.Database!.UpdateAsync(premio);
        }

        public async Task<bool> ChangePremioStatusAsync(int id)
        {
            var premio = await GetPremioByIdAsync(id);
            if (premio == null) return false;

            premio.EstadoPremio = !premio.EstadoPremio;
            await _database.Database!.UpdateAsync(premio);
            return true;
        }
    }
}
