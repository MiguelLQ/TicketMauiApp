using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories
{
    public class PremioRepository : IPremioRepository
    {
        public Task EliminarAsync(Premio premio)
        {
            throw new NotImplementedException();
        }

        public Task GuardarAsync(Premio premio)
        {
            throw new NotImplementedException();
        }

        public Task<Premio> ObtenerPorIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Premio>> ObtenerTodosAsync()
        {
            throw new NotImplementedException();
        }
    }
}
