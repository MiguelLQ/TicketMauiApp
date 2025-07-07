using SQLite;

namespace MauiFirebase.Models;
public class User
{
    [PrimaryKey,AutoIncrement]
    public int IdUser { get; set; }
    [Indexed]
    public int IdRol { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordUser { get; set; } = string.Empty;
    public bool EstadoUser { get; set; }
    public DateTime FechaRegistroUser { get; set; }
}
