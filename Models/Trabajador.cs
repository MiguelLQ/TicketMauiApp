using SQLite;

namespace MauiFirebase.Models;
public class Trabajador
{
    [PrimaryKey,AutoIncrement]
    public int IdTrabajador { get; set; }
    [Indexed]
    public int IdUsuario { get; set; }
    public string NombreTrabajador { get; set; } = string.Empty;
    public string ApellidoTrabajador { get; set; } = string.Empty;
    public string DniTrabajador { get; set; } = string.Empty;
    public string CorreoTrabajador { get; set; } = string.Empty;
    public string TelefonoTrabajador { get; set; } = string.Empty;
    public bool EstadoTrabajador { get; set; }
    public DateTime FechaRegistroTrabajador { get; set; }
}
