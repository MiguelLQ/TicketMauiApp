using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiFirebase.Models;

public class Residente
{
    [PrimaryKey, AutoIncrement]
    public int IdResidente { get; set; }
    [Indexed]
    public int? IdUsuario { get; set; }
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

}
