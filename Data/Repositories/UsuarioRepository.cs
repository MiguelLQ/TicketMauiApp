using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
using MauiFirebase.Services;
using SQLite;

namespace MauiFirebase.Data.Repositories
{
    public class UsuarioRepository:IUsuarioRepository
    {
        private readonly FirebaseUsuarioService _firebaseService = new();
        private readonly SQLiteAsyncConnection _db;

        public UsuarioRepository()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "AppData.db3");
            var database = new AppDatabase(dbPath);
            _db = database.Database!;
        }
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            var token = Preferences.Get("FirebaseToken", string.Empty);
            if (string.IsNullOrEmpty(token))
            {
                // Sin token, cargar desde SQLite
                return await _db.Table<Usuario>().ToListAsync();
            }

            try
            {
                // Obtener desde Firebase
                var usuariosFirebase = await _firebaseService.ObtenerUsuariosDesdeFirestoreAsync(token);

                //Guardar localmente
                await _db.DeleteAllAsync<Usuario>(); // Limpia y sincroniza
                await _db.InsertAllAsync(usuariosFirebase);

                return usuariosFirebase;
            }
            catch (Exception ex)
            {
                //Error al conectar, usar datos locales
                Console.WriteLine($"Error Firebase: {ex.Message}");
                return await _db.Table<Usuario>().ToListAsync();
            }
        }
        public async Task<string?> AgregarUsuarioAsync(Usuario usuario)
        {
            var token = Preferences.Get("FirebaseToken", string.Empty);
            if (string.IsNullOrEmpty(token))
                return null;

            var uid = await _firebaseService.AgregarUsuarioAsync(usuario, token);

            if (!string.IsNullOrEmpty(uid))
            {
                usuario.Uid = uid;
                await _db.InsertAsync(usuario);
                return uid;
            }

            return null;
        }

    }
}
