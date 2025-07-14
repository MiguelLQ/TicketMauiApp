using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces
{
    public interface ICategoriaResiduoRepository
    {
        //metodos para categoriaresiduo
        Task<List<CategoriaResiduo>> GetAllCategoriaResiduoAsync();
        Task<CategoriaResiduo> CreateCategoriaResiduoAsync(CategoriaResiduo categoriaResiduo);
        Task<CategoriaResiduo?> GetCategoriaResiduoIdAsync(string id);
        Task<int> UpdateCategoriaResiduoAsync(CategoriaResiduo categoriaResiduo);
        Task<bool> ChangeEstadoCategoriaResiduoAsync(string id);
        Task MarcarComoSincronizadoAsync(string id);
        Task<List<CategoriaResiduo>> GetCategoriasNoSincronizadasAsync();
        Task<bool> ExisteAsync(string id);
    }
}
