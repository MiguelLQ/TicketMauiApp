using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces
{
    public interface IPremioRepository
    {
        Task<List<Premio>> ObtenerTodosAsync();
        Task<Premio> ObtenerPorIdAsync(int id);
        Task GuardarAsync(Premio premio);
        Task EliminarAsync(Premio premio);
    }
}
