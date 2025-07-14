using MauiFirebase.Data.Interfaces;
using MauiFirebase.Data.Sources;
using MauiFirebase.Models;

namespace MauiFirebase.Data.Repositories;

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

    public async Task<Ticket?> GetTicketIdAsync(string id)
    {
        return await _database.Database!.Table<Ticket>().FirstOrDefaultAsync(t => t.IdTicket == id);
    }

    public async Task<int> UpdateTicketAsync(Ticket ticket)
    {
        return await _database.Database!.UpdateAsync(ticket);
    }

    public async Task<bool> ChangeEstadoTicketAsync(string id)
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

    public async Task MarcarComoSincronizadoAsync(string id)
    {
        var ticket = await GetTicketIdAsync(id);
        if (ticket != null)
        {
            ticket.Sincronizado = true;
            await _database.Database!.UpdateAsync(ticket);
        }
    }

    public async Task<List<Ticket>> GetTicketsNoSincronizadosAsync()
    {
        return await _database.Database!.Table<Ticket>().Where(t => !t.Sincronizado).ToListAsync();
    }

    public async Task<bool> ExisteAsync(string id)
    {
        var ticket = await GetTicketIdAsync(id);
        return ticket != null;
    }
}
