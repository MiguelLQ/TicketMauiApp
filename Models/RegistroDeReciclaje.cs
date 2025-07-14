using SQLite;

namespace MauiFirebase.Models;

public class RegistroDeReciclaje
{
    [PrimaryKey]

    public string IDRegistroDeReciclaje { get; set; } = Guid.NewGuid().ToString();
    [Indexed]
    public string? IdResidente { get; set; }
    public string? IdResiduo { get; set; }
    [NotNull]
    public decimal PesoKilogramo { get; set; }
    [NotNull]
    public DateTime FechaRegistro { get; set; }
    [NotNull]
    public int TicketsGanados { get; set; }
    [Ignore]
    public string? NombreResidente { get; set; }
    [Ignore]
    public string? ApellidoResidente { get; set; }
    [Ignore]
    public string? DniResidente { get; set; }
    [Ignore]
    public string? NombreResiduo { get; set; }
    public bool Sincronizado { get; set; } = false;
}

public class ReciclajePorCategoria
{
    public required string Categoria { get; set; }
    public decimal TotalKg { get; set; }
}



