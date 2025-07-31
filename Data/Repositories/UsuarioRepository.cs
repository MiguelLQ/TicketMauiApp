using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
using SQLite;

namespace MauiFirebase.Data.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly FirebaseUbicacionServie _firebaseService = new();
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
        if (string.IsNullOrWhiteSpace(usuario.Correo) || string.IsNullOrWhiteSpace(usuario.Contraseña))
        {
            Console.WriteLine("❌ Correo o contraseña vacíos.");
            return false;
        }

        // 1. Crear en Firebase Auth
        var uid = await _authService.RegistrarAuthUsuarioAsync(usuario.Correo, usuario.Contraseña);
        Console.WriteLine($"✅ UID obtenido: {uid}");

        if (string.IsNullOrEmpty(uid))
        {
            Console.WriteLine("❌ Error al registrar en Firebase Auth.");
            return false;
        }

        // 2. Obtener token
        var token = await _authService.ObtenerIdTokenSeguroAsync();
        Console.WriteLine($"✅ Token obtenido: {token}");

        // 3. Guardar en Firestore
        usuario.Uid = uid;
        var resultado = await _firebaseService.AgregarUsuarioAsync(usuario, token);
        Console.WriteLine($"✅ Resultado Firestore: {resultado}");

        if (!string.IsNullOrEmpty(resultado))
        {
            await _db.InsertAsync(usuario);
            Console.WriteLine("✅ Usuario guardado localmente.");
            return true;
        }

        Console.WriteLine("❌ Error al guardar en Firestore.");
        return false;
    }

    public async Task<Usuario?> ObtenerUsuarioPorUidAsync(string uid)
    {
        return await _db.Table<Usuario>().FirstOrDefaultAsync(u => u.Uid == uid);
    }


    public async Task<bool> EditarUsuarioLocalAsync(Usuario usuario)
    {
        var resultado = await _db.UpdateAsync(usuario);
        return resultado > 0;
    }

}
