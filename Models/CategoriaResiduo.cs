using SQLite;

namespace MauiFirebase.Models;

public class CategoriaResiduo
{
    [PrimaryKey,AutoIncrement]
    public int IdCategoriaResiduo { get; set; }
    [Indexed]
    public int IdTicket { get; set; }
    [NotNull]
    public string? NombreCategoria { get; set; }
    [NotNull]
    public bool EstadoCategoriaResiduo { get; set; }
}
