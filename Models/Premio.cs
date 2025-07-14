using SQLite;

namespace MauiFirebase.Models;
public class Premio
{

    [PrimaryKey]
    public string IdPremio { get; set; }= Guid.NewGuid().ToString();
    public string NombrePremio { get; set; } = string.Empty;
    public string DescripcionPremio { get; set; } = string.Empty;
    public int PuntosRequeridos { get; set; }
    public bool EstadoPremio { get; set; }
    public string FotoPremioUrl{ get; set; } = string.Empty;
    public string FotoPremio { get; set; } = string.Empty;

}