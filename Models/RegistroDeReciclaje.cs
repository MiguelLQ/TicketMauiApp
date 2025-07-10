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
    public string? ApellidoResidente { get; set; }
    [Ignore]
    public string? DniResidente { get; set; }
    [Ignore]
    public string? NombreResiduo { get; set; }

}

public class ReciclajePorCategoria
{
    public required string Categoria { get; set; }
    public decimal TotalKg { get; set; }
}



