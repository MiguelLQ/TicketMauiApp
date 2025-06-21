using MauiFirebase.Data.Interfaces;
using MauiFirebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiFirebase.Data.Repositories
{
    class CanjeRepository : ICanjeRepository

    {

        public Task<bool> ChangeEstadoCanjeAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Canje> CreateCanjeAsync(Canje canje)
        {
            throw new NotImplementedException();
        }

        public Task<List<Canje>> GetAllCanjeAync()
        {
            throw new NotImplementedException();
        }

        public Task<Canje?> GetCanjeIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateCanjeAsync(Canje canje)
        {
            throw new NotImplementedException();
        }
    }
}
