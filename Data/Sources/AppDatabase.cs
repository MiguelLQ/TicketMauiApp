using MauiFirebase.Models;
using Microsoft.Maui.Animations;
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
        _database.CreateTableAsync<Convertidor>().Wait();
        _database.CreateTableAsync<Ticket>().Wait();
        _database.CreateTableAsync<RegistroDeReciclaje>().Wait();
        _database.CreateTableAsync<CategoriaResiduo>().Wait();
        _database.CreateTableAsync<Canje>().Wait();
        _database.CreateTableAsync<Residente>().Wait();
        _database.CreateTableAsync<Premio>().Wait();
        



    }
}
