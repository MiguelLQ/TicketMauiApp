using MauiFirebase.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MauiFirebase.Data.Interfaces;
public interface ITrabajadorRepository
{
    Task<List<Trabajador>> GetAllTrabajadorAsync();
    Task<Trabajador> CreateTrabajadorAsync(Trabajador trabajador);
    Task<Trabajador?> GetTrabajadorByIdAsync(int id);
    Task<int> UpdateTrabajadorAsync(Trabajador trabajador);
    Task<bool> ChangeEstadoTrabajadorAsync(int id);
}
