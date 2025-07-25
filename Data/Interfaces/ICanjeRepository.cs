﻿using MauiFirebase.Models;
using System.Collections.ObjectModel;

namespace MauiFirebase.Data.Interfaces
{
    public interface ICanjeRepository
    {
        Task<List<Canje>> GetAllCanjeAync();
        Task<Canje> CreateCanjeAsync(Canje canje);
        Task<Canje?> GetCanjeIdAsync(string id);
        Task<int> UpdateCanjeAsync(Canje canje);
        Task<bool> ChangeEstadoCanjeAsync(string id);
        
        Task<Canje?> ObtenerPorUidAsync(string uid);
        Task MarcarComoSincronizadoAsync(string id);
        Task<List<Canje>> GetCanjesNoSincronizadosAsync();
        Task<bool> ExisteAsync(string id);
    }
}
