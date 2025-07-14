using SQLite;
namespace MauiFirebase.Models;
public class Convertidor
{
    [PrimaryKey]
    public string IdConvertidor { get; set; } = Guid.NewGuid().ToString();
    [NotNull]
    public int ValorMin { get; set; }
    [NotNull]
    public int ValorMax { get; set; }
    [NotNull]
    public int NumeroTicket { get; set; }
    public bool EstadoConvertidor { get; set; }
    public bool Sincronizado { get; set; } = false;//para offline
}
