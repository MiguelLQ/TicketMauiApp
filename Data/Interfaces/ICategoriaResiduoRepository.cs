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
        Task<CategoriaResiduo?> GetCategoriaResiduoIdAsync(int id);
        Task<int> UpdateCategoriaResiduoAsync(CategoriaResiduo categoriaResiduo);
        Task<bool> ChangeEstadoCategoriaResiduoAsync(int id);

    }
}
