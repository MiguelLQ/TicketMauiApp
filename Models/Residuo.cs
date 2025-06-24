using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiFirebase.Models;
public class Residuo
{
    [PrimaryKey, AutoIncrement]
    public int IdResiduo { get; set; }
    [Indexed]
    public int IdCategoriaResiduo { get; set; }
    [NotNull]
    public string? NombreResiduo { get; set; }
    [NotNull]
    public bool EstadoResiduo { get; set; } = true;
    [Ignore] // SQLite ignora esta propiedad
    public string? NombreCategoria { get; set; }
}
