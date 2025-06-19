using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.Data.Interfaces
{
    public interface ICanjeRepository
    {
        Task<List<Canje>> GetAllCanjeAync();
        Task<Canje> CreateCanjeAsync(Canje canje);
        Task<Canje?> GetCanjeIdAsync(int id);
        Task<int> UpdateCanjeAsync(Canje canje);
        Task<bool> ChangeEstadoCanjeAsync(int id);
    }
}
