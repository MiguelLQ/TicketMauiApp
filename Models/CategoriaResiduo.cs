using SQLite;

namespace MauiFirebase.Models;

public class CategoriaResiduo
{
    [PrimaryKey]
    public string IdCategoriaResiduo { get; set; } = Guid.NewGuid().ToString();
    [Indexed]
    public string? IdTicket { get; set; }
    [NotNull]
    public string? NombreCategoria { get; set; }
    [NotNull]
    public bool EstadoCategoriaResiduo { get; set; }
    [Ignore] 
    public Ticket? Ticket { get; set; }
    // Indica si el registro está sincronizado con Firestore
    public bool Sincronizado { get; set; } = false;
}
