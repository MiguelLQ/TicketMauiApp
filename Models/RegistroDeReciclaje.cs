using SQLite;

namespace MauiFirebase.Models;

public class RegistroDeReciclaje
{
    [PrimaryKey, AutoIncrement]

    public int IDRegistroDeReciclaje { get; set; }
    [Indexed]
    public int IdResidente { get; set; }
    [NotNull]
    public int IdResiduo { get; set; }
    [NotNull]
    public decimal PesoKilogramo { get; set; }
    [NotNull]
    public DateTime FechaRegistro { get; set; }
    [NotNull]
    public int TicketsGanados { get; set; }
    [Ignore] // SQLite ignora esta propiedad
    public string? NombreResidente { get; set; }
    [Ignore] // SQLite ignora esta propiedad
    public string? NombreResiduo { get; set; }
}


