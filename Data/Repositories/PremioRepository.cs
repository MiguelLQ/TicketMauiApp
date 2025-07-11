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
        private readonly FirebasePremioService _firebaseService;
        private readonly FirebaseAuthService _authService;

        public PremioRepository(AppDatabase database)
        {
            _database = database;
            _firebaseService = new FirebasePremioService();
            _authService = new FirebaseAuthService(); // 👈 Solo si no lo inyectas
            _ = _database.Database!.CreateTableAsync<Premio>(); // asegúrate de crear la tabla

        }

        public async Task<Premio> CreatePremioAsync(Premio premio)
        {
            // 1. Guardar localmente (SQLite) para obtener el IdPremio generado automáticamente
            await _database.Database!.InsertAsync(premio);

            // 2. Obtener ID Token del usuario autenticado
            string idToken = await _authService.ObtenerIdTokenSeguroAsync();

            // 3. Definir el ID remoto como el IdPremio local (Firestore lo permite)
            string idRemoto = premio.IdPremio.ToString();

            // 4. Subir a Firestore
            await _firebaseService.GuardarPremioFirestoreAsync(premio, idRemoto, idToken);

            return premio;
        }
        public async Task SincronizarDesdeFirestoreAsync()
        {
            var idToken = await _authService.ObtenerIdTokenSeguroAsync();
            var premiosRemotos = await _firebaseService.ObtenerPremiosDesdeFirestoreAsync(idToken);
            var premiosLocales = await GetAllPremiosAsync();
            var idsLocales = premiosLocales.Select(p => p.IdPremio).ToHashSet();

            foreach (var premio in premiosRemotos)
            {
                if (!idsLocales.Contains(premio.IdPremio))
                {
                    // 🟡 Descargar imagen si tiene URL
                    if (!string.IsNullOrEmpty(premio.FotoPremioUrl))
                    {
                        premio.FotoPremio = await DescargarImagenLocalAsync(premio.FotoPremioUrl);
                    }

                    await _database.Database!.InsertAsync(premio);
                }
            }
        }

        public async Task<Premio> CreatePremioLocalAsync(Premio premio)
        {
            await _database.Database!.InsertAsync(premio);
            return premio;
        }
        public async Task<string> DescargarImagenLocalAsync(string urlRemota)
        {
            var httpClient = new HttpClient();
            var data = await httpClient.GetByteArrayAsync(urlRemota);

            var nombreArchivo = Path.GetFileName(new Uri(urlRemota).LocalPath);
            var rutaCarpeta = Path.Combine(FileSystem.AppDataDirectory, "imagenes");

            if (!Directory.Exists(rutaCarpeta))
                Directory.CreateDirectory(rutaCarpeta);

            var rutaLocal = Path.Combine(rutaCarpeta, nombreArchivo);
            await File.WriteAllBytesAsync(rutaLocal, data);

            return rutaLocal;
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
        public async Task<int> ObtenerCantidadPremios()
        {
            var premios = await _database.Database!.Table<Premio>().ToListAsync();
            return premios.Count;
        }
    }
}
