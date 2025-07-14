using MauiFirebase.Models;
namespace MauiFirebase.Data.Interfaces;
public interface ITicketRepository
{
    Task<List<Ticket>> GetAllTicketAync();
    Task<Ticket> CreateTicketAsync(Ticket ticket);
    Task<Ticket?> GetTicketIdAsync(string id);
    Task<int> UpdateTicketAsync(Ticket ticket);
    Task<bool> ChangeEstadoTicketAsync(string id);
    Task MarcarComoSincronizadoAsync(string id);
    Task<List<Ticket>> GetTicketsNoSincronizadosAsync();
    Task<bool> ExisteAsync(string id);
}
