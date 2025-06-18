using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories;
public class ResiduoRepository : IResiduoRepository
{
    public Task<bool> ChangeEstadoResiduoAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Residuo> CreateResiduoAsync(Residuo residuo)
    {
        throw new NotImplementedException();
    }

    public Task<List<Residuo>> GetAllResiduoAync()
    {
        throw new NotImplementedException();
    }

    public Task<Residuo?> GetResiduoIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateResiduoAsync(Residuo residuo)
    {
        throw new NotImplementedException();
    }
}
