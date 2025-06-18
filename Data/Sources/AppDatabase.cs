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
        _database.CreateTableAsync<Residuo>().Wait();
    }
}
