using SQLite;

namespace MauiFirebase.Models;

public class Ticket
{
    [PrimaryKey]
    public string IdTicket { get; set; }= Guid.NewGuid().ToString();
    [NotNull]
    public string? ColorTicket { get; set; }
    [NotNull]
    public bool EstadoTicket { get; set; } = true;
    [NotNull]
    public DateTime FechaRegistro { get; set; }= DateTime.Now;

    // Indica si el registro está sincronizado con Firestore
    public bool Sincronizado { get; set; } = false;
}
