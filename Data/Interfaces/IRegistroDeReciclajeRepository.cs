using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces
{
    public interface IRegistroDeReciclajeRepository
    {
        Task<List<RegistroDeReciclaje>> ObtenerTodosAsync();      
        Task<RegistroDeReciclaje?> ObtenerPorIdAsync(int id);     
        Task GuardarAsync(RegistroDeReciclaje registro);          
        Task EliminarAsync(int id);
        Task<decimal> ObtenerTotalRecicladoKg();
    }
}
