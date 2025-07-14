
using SQLite;
using System.Text.Json.Serialization;

namespace MauiFirebase.Models;
public class Vehiculo
{
    [PrimaryKey]
    public string IdVehiculo { get; set; }= Guid.NewGuid().ToString();
    [Indexed]
    public string? IdUsuario { get; set; }
    public string PlacaVehiculo { get; set; } = string.Empty;
    public string MarcaVehiculo { get; set; } = string.Empty;
    public string ModeloVehiculo { get; set; } = string.Empty;
    public bool EstadoVehiculo { get; set; }
    public DateTime FechaRegistroVehiculo { get; set; }
    [JsonIgnore]
    public string Nombre { get; set; } = string.Empty;
    public bool Sincronizado { get; set; } = false;
   

}
