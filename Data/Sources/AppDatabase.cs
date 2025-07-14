using MauiFirebase.Models;
using SQLite;

namespace MauiFirebase.Data.Sources;
public class AppDatabase
{
    private readonly SQLiteAsyncConnection _database;
    public SQLiteAsyncConnection? Database { get; } 

    public AppDatabase(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath); 
        Database = _database;
        InitializeTables();
    }

    private void InitializeTables()
    {
        /*======================================================  
          Aquí se agrega más tablas  
         =========================================================*/
        _database.CreateTableAsync<UbicacionVehiculo>().Wait();
        _database.CreateTableAsync<Residuo>().Wait();
        _database.CreateTableAsync<Convertidor>().Wait();
        _database.CreateTableAsync<Ticket>().Wait();
        _database.CreateTableAsync<RegistroDeReciclaje>().Wait();
        _database.CreateTableAsync<CategoriaResiduo>().Wait();
        _database.CreateTableAsync<Canje>().Wait();
        _database.CreateTableAsync<Residente>().Wait();
        _database.CreateTableAsync<Premio>().Wait();
        _database.CreateTableAsync<Vehiculo>().Wait();
        _database.CreateTableAsync<Usuario>().Wait();
        _database.CreateTableAsync<Ruta>().Wait();
    }

    internal IEnumerable<object> Table<T>()
    {
        throw new NotImplementedException();
    }
}
