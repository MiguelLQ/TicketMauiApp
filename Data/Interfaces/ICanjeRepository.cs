using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.Data.Interfaces
{
    internal interface ICanjeRepository
    {
        Task<List<Canje>> GetAllResiduoAync();
        Task<Canje> CreateResiduoAsync(Canje canje);
        Task<Residuo?> GetResiduoIdAsync(int id);
        Task<int> UpdateResiduoAsync(Residuo residuo);
        Task<bool> ChangeEstadoResiduoAsync(int id);
    }
}
