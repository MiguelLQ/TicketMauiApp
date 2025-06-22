using System.Collections.Generic;
using System.Threading.Tasks;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces
{
    public interface IPremioRepository
    {
        Task<List<Premio>> GetAllPremiosAsync();
        Task<Premio> CreatePremioAsync(Premio premio);
        Task<Premio?> GetPremioByIdAsync(int id);
        Task<int> UpdatePremioAsync(Premio premio);
        Task<bool> ChangePremioStatusAsync(int id);
    }
}
