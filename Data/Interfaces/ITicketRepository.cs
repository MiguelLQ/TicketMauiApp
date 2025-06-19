using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Interfaces
{
    public interface ITicketRepository
    {
        //metodos para ticket
        Task<List<Ticket>> GetAllTicketAync();
        Task<Ticket> CreateTicketAsync(Ticket ticket);
        Task<Ticket?> GetTicketIdAsync(int id);
        Task<int> UpdateTicketAsync(Ticket ticket);
        Task<bool> ChangeEstadoTicketAsync(int id);
    }
}
