using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces
{
    public interface IRegistroDeReciclajeRepository
    {
        Task<List<RegistroDeReciclaje>> ObtenerTodosAsync();      
        Task<RegistroDeReciclaje?> ObtenerPorIdAsync(string id);     
        Task GuardarAsync(RegistroDeReciclaje registro);          
        Task EliminarAsync(string id);
        Task<decimal> ObtenerTotalRecicladoKg();
        //para torta 
        Task<List<ReciclajePorCategoria>> ObtenerTotalesPorCategoriaAsync();
        Task<List<RegistroDeReciclaje>> UltimosTresRegistros();
        Task MarcarComoSincronizadoAsync(string id);
        Task<List<RegistroDeReciclaje>> GetRegistrosNoSincronizadosAsync();
        Task<bool> ExisteAsync(string id);

    }
}
