using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;
using SQLite;

namespace MauiFirebase.Data.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDatabase _database;

        public TicketRepository(AppDatabase database)
        {
            _database = database;
            _ = _database.Database!.CreateTableAsync<Ticket>(); // asegúrate de crear la tabla
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            ticket.FechaRegistro = DateTime.Now;
            await _database.Database!.InsertAsync(ticket);
            return ticket;
        }

        public async Task<List<Ticket>> GetAllTicketAync()
        {
            return await _database.Database!.Table<Ticket>().ToListAsync();
        }

        public async Task<Ticket?> GetTicketIdAsync(int id)
        {
            return await _database.Database!.Table<Ticket>().FirstOrDefaultAsync(t => t.IdTicket == id);
        }

        public async Task<int> UpdateTicketAsync(Ticket ticket)
        {
            return await _database.Database!.UpdateAsync(ticket);
        }

        public async Task<bool> ChangeEstadoTicketAsync(int id)
        {
            Ticket ticket = await _database.Database!.Table<Ticket>()
                             .Where(r => r.IdTicket == id)
                             .FirstOrDefaultAsync();
            if (ticket == null)
            {
                return false;
            }
            ticket.EstadoTicket = !ticket.EstadoTicket;
            int result = await _database.Database.UpdateAsync(ticket);
            return result > 0;
        }
    }

}
