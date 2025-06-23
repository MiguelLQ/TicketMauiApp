using MauiFirebase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiFirebase.Data.Interfaces
{
    public interface IResidenteRepository
    {
        // Métodos para Residentes

        Task<List<Residente>> GetAllResidentesAsync();


        /// <returns>Una tarea que devuelve el objeto Residente creado, con su ID asignado.</returns>
        Task<Residente> CreateResidenteAsync(Residente residente);


        /// <returns>Una tarea que devuelve el objeto Residente si se encuentra, o null si no existe.</returns>
        Task<Residente?> GetResidenteByIdAsync(int id); // Usamos string porque IdResidente es string


        /// <returns>Una tarea que devuelve el número de filas afectadas (normalmente 1 si la actualización fue exitosa).</returns>
        Task<int> UpdateResidenteAsync(Residente residente);


        /// <returns>Una tarea que devuelve true si el estado se cambió con éxito, false en caso contrario.</returns>
        Task<bool> ChangeEstadoResidenteAsync(int id); // Usamos string porque IdResidente es string


        /// <returns>Una tarea que devuelve una lista de residentes que coinciden con la búsqueda.</returns>
        Task<List<Residente>> SearchResidentesByNameOrApellidoAsync(string searchText);


        /// <returns>Una tarea que devuelve el objeto Residente si se encuentra, o null si no existe.</returns>
        Task<Residente?> GetResidenteByDniAsync(string dni);
        Task<Residente> ObtenerPorDniAsync(string dniResidente);
    }
}
