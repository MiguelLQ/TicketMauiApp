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
    class CanjeRepository : ICanjeRepository
    {
        private readonly AppDatabase _database;

        public CanjeRepository(AppDatabase database)
        {
            _database = database;
            _ = _database.Database!.CreateTableAsync<Canje>();
        }

        public async Task<Canje> CreateCanjeAsync(Canje canje)
        {
            canje.FechaCanje = DateTime.Now;
            await _database.Database!.InsertAsync(canje);
            return canje;
        }

        public async Task<List<Canje>> GetAllCanjeAync()
        {
            return await _database.Database!.Table<Canje>().ToListAsync();
        }

        public async Task<Canje?> GetCanjeByIdAsync(int id)
        {
            return await _database.Database!.Table<Canje>()
                .FirstOrDefaultAsync(c => c.IdCanje == id);
        }

        public async Task<int> UpdateCanjeAsync(Canje canje)
        {
            return await _database.Database!.UpdateAsync(canje);
        }

        public async Task<bool> ChangeEstadoCanjeAsync(int id)
        {
            var canje = await _database.Database!.Table<Canje>()
                .FirstOrDefaultAsync(c => c.IdCanje == id);
            if (canje == null)
                return false;

            canje.EstadoCanje = !canje.EstadoCanje;
            int result = await _database.Database.UpdateAsync(canje);
            return result > 0;
        }

        public Task<Canje?> GetCanjeIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
