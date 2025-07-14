using System.Collections.Generic;
using System.Threading.Tasks;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces;

public interface IPremioRepository
{
    Task<List<Premio>> GetAllPremiosAsync();
    Task<Premio> CreatePremioAsync(Premio premio);
    Task SincronizarDesdeFirestoreAsync();
    Task<Premio> CreatePremioLocalAsync(Premio premio);
    Task<string> DescargarImagenLocalAsync(string urlRemota);
    Task<Premio?> GetPremioByIdAsync(string id);
    Task<int> UpdatePremioAsync(Premio premio);
    Task<bool> ChangePremioStatusAsync(string id);
    Task<int> ObtenerCantidadPremios();
}
