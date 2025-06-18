using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Models;
using SQLite;

namespace MauiFirebase.Data.Sources
{
    public class SQLiteDataSource
    {
        private readonly SQLiteAsyncConnection _database;

        public SQLiteDataSource(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Premio>().Wait();
        }

        public Task<List<Premio>> ObtenerTodosAsync() => _database.Table<Premio>().ToListAsync();

        public Task<Premio> ObtenerPorIdAsync(int id) =>
            _database.Table<Premio>().Where(p => p.Id == id).FirstOrDefaultAsync();

        public Task<int> GuardarAsync(Premio premio) =>
            premio.Id == 0 ? _database.InsertAsync(premio) : _database.UpdateAsync(premio);

        public Task<int> EliminarAsync(Premio premio) => _database.DeleteAsync(premio);

        internal async Task EliminarAsync(RegistroDeReciclaje registro)
        {
            throw new NotImplementedException();
        }

        internal async Task GuardarAsync(RegistroDeReciclaje registro)
        {
            throw new NotImplementedException();
        }

        internal Task<T> ObtenerPorIdAsync<T>(int id)
        {
            throw new NotImplementedException();
        }

        internal Task<List<T>> ObtenerTodosAsync<T>()
        {
            throw new NotImplementedException();
        }
    }
}
