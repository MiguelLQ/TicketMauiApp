using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories
{
    public class RegistroDeReciclajeRepository : IRegistroDeReciclajeRepository
    {
        private readonly SQLiteDataSource _local;

        public RegistroDeReciclajeRepository(SQLiteDataSource local)
        {
            _local = local;
        }

        public Task EliminarAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task GuardarAsync(RegistroDeReciclaje registro)
        {
            await _local.GuardarAsync(registro);
            // TODO: Sincronizar con Firebase si se desea
        }

        public async Task<RegistroDeReciclaje?> ObtenerPorIdAsync(int id)
        {
            return await _local.ObtenerPorIdAsync<RegistroDeReciclaje>(id);
        }

        public async Task<List<RegistroDeReciclaje>> ObtenerTodosAsync()
        {
            return await _local.ObtenerTodosAsync<RegistroDeReciclaje>();
        }
    }
}
