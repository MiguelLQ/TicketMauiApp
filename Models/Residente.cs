using SQLite;
namespace MauiFirebase.Models;

public class Residente
{
    [PrimaryKey]
    public string IdResidente { get; set; }= Guid.NewGuid().ToString();
    [NotNull]
    public string? NombreResidente { get; set; }
    [NotNull]
    public string? ApellidoResidente { get; set; }
    public string? DniResidente { get; set; }
    [NotNull]
    public string? CorreoResidente { get; set; }
    [NotNull]
    public string? DireccionResidente { get; set; }
    [NotNull]
    public bool EstadoResidente { get; set; }
    [NotNull]
    public DateTime FechaRegistroResidente { get; set; } = DateTime.Now;
    [NotNull]
    public int TicketsTotalesGanados { get; set; } = 0;
    public bool Sincronizado { get; set; } = false;
}
