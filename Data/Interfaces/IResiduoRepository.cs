using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.Data.Interfaces;
public interface ICategoriaRepository
{
    Task<List<Residuo>> GetAllResiduoAync();
    Task<Residuo> CreateResiduoAsync(Residuo residuo);
    Task<Residuo?> GetResiduoIdAsync(int id);
    Task<int> UpdateResiduoAsync(Residuo residuo);
    Task<bool> ChangeEstadoResiduoAsync(int id);
}
