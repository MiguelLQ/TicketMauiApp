using SQLite;
namespace MauiFirebase.Models;
public class Residuo
{
    [PrimaryKey]
    public string IdResiduo { get; set; }= Guid.NewGuid().ToString();
    [Indexed]
    public string? IdCategoriaResiduo { get; set; }
    [NotNull]
    public string? NombreResiduo { get; set; }
    [NotNull]
    public bool EstadoResiduo { get; set; } = true;
    public decimal ValorResiduo { get; set; } // Cambiado de int a decimal
    [Ignore] 
    public string? NombreCategoria { get; set; }
    // Indica si el registro está sincronizado con Firestore
    public bool Sincronizado { get; set; } = false;
    public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;

}
