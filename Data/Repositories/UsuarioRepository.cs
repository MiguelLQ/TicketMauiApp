using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
using MauiFirebase.Services;
using SQLite;

namespace MauiFirebase.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly FirebaseUsuarioService _firebaseService = new();
        private readonly FirebaseAuthService _authService = new();
        private readonly SQLiteAsyncConnection _db;

        public UsuarioRepository()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "AppData.db3");
            var database = new AppDatabase(dbPath);
            _db = database.Database!;
        }

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            var token = await _authService.ObtenerIdTokenSeguroAsync();

            if (string.IsNullOrEmpty(token))
            {
                // No hay token: cargar desde SQLite
                return await _db.Table<Usuario>().ToListAsync();
            }

            try
            {
                var usuariosFirebase = await _firebaseService.ObtenerUsuariosDesdeFirestoreAsync(token);

                // Sincronizar localmente
                await _db.DeleteAllAsync<Usuario>();
                await _db.InsertAllAsync(usuariosFirebase);

                return usuariosFirebase;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Firebase: {ex.Message}");
                return await _db.Table<Usuario>().ToListAsync(); // fallback local
            }
        }

        public async Task<bool> AgregarUsuarioAsync(Usuario usuario)
        {
            var token = await _authService.ObtenerIdTokenSeguroAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            var uid = await _firebaseService.AgregarUsuarioAsync(usuario, token);

            if (!string.IsNullOrEmpty(uid))
            {
                usuario.Uid = uid;
                await _db.InsertAsync(usuario); // Guardar local
                return true;
            }

            return false;
        }

    }
}
