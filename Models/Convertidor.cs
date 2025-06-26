
using SQLite;

namespace MauiFirebase.Models;
public class Convertidor
{
    [PrimaryKey, AutoIncrement]
    public int IdConvertidor { get; set; }
    public int ValorMin { get; set; }
    public int ValorMax { get; set; }
    public int NumeroTicket { get; set; }
}
