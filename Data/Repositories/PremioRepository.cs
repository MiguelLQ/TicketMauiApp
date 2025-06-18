using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories
{
    public class PremioRepository : IPremioRepository
    {
        private readonly SQLiteDataSource _local;
        public PremioRepository(SQLiteDataSource local)
        {
            _local = local;
        }

        public async  Task EliminarAsync(Premio premio)
        {
            await _local.EliminarAsync(premio);
        // TODO: Eliminar también en Firebase si hay conexión
           }
        public async Task GuardarAsync(Premio premio)
        {
            await _local.GuardarAsync(premio);
        // TODO: Aquí irá sincronización con Firebase
        }
        public async Task<Premio> ObtenerPorIdAsync(int id)
        {
            return await _local.ObtenerPorIdAsync(id);
        }

        public async Task<List<Premio>> ObtenerTodosAsync()
        {
            return await _local.ObtenerTodosAsync();
        }
    }
}
