using SQLite;
namespace MauiFirebase.Models;
public class Convertidor
{
    [PrimaryKey, AutoIncrement]
    public int IdConvertidor { get; set; }
    [NotNull]
    public int ValorMin { get; set; }
    [NotNull]
    public int ValorMax { get; set; }
    [NotNull]
    public int NumeroTicket { get; set; }
    public bool EstadoConvertidor { get; set; }
}
