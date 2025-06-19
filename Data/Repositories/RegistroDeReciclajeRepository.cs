using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories
{
    public class RegistroDeReciclajeRepository : IRegistroDeReciclajeRepository
    {
        public Task EliminarAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task GuardarAsync(RegistroDeReciclaje registro)
        {
            throw new NotImplementedException();
        }

        public Task<RegistroDeReciclaje?> ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<RegistroDeReciclaje>> ObtenerTodosAsync()
        {
            throw new NotImplementedException();
        }
    }
}
