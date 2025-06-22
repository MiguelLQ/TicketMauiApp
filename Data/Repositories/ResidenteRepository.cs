using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
using SQLite;

namespace MauiFirebase.Data.Repositories
{
    public class ResidenteRepository : IResidenteRepository
    {
        private readonly AppDatabase _database;

        /// <param name="database">La instancia de AppDatabase para la conexión SQLite.</param>
        public ResidenteRepository(AppDatabase database)
        {
            _database = database;
            // Asegúrate de que la tabla Residente exista.
            // Simplemente llama al método y espera, sin el '_ ='
            _database.Database!.CreateTableAsync<Residente>().Wait(); // ¡Cambio aquí!
        }

        /// <param name="residente">El objeto Residente a insertar.</param>

        public async Task<Residente> CreateResidenteAsync(Residente residente)
        {
            await _database.Database!.InsertAsync(residente);
            return residente;
        }

        public async Task<List<Residente>> GetAllResidentesAsync()
        {
            return await _database.Database!.Table<Residente>().ToListAsync();
        }

        /// <param name="id">El IdResidente del residente a buscar.</param>

        public async Task<Residente?> GetResidenteByIdAsync(int id)
        {
            return await _database.Database!.Table<Residente>()
                                            .Where(r => r.IdResidente == id) // Ahora IdResidente también es int
                                            .FirstOrDefaultAsync();
        }


        /// <param name="residente">El objeto Residente con los datos actualizados.</param>

        public async Task<int> UpdateResidenteAsync(Residente residente)
        {
            return await _database.Database!.UpdateAsync(residente);
        }

        /// <param name="id">El IdResidente del residente cuyo estado se va a cambiar.</param>

        public async Task<bool> ChangeEstadoResidenteAsync(int id) // O string id, según tu IdResidente
        {
            if (id <= 0)
                return false;

            Residente? residente = await _database.Database!.Table<Residente>()
                                                   .Where(r => r.IdResidente == id)
                                                   .FirstOrDefaultAsync();

            if (residente == null)
            {
                return false; // Residente no encontrado
            }

            // --- Lógica para alternar el estado booleano ---
            residente.EstadoResidente = !residente.EstadoResidente; // Simplemente invierte el valor

            int result = await _database.Database!.UpdateAsync(residente);
            return result > 0; // Retorna true si la actualización fue exitosa
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

        /// <param name="dni">El DNI a buscar.</param>

        public async Task<Residente?> GetResidenteByDniAsync(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni))
            {
                return null;
            }

            return await _database.Database!.Table<Residente>()
                                            .Where(r => r.DniResidente == dni) // Búsqueda exacta para DNI
                                            .FirstOrDefaultAsync();
        }
    }
}