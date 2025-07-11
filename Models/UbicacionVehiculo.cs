using SQLite;
using System.Text.Json.Serialization;

namespace MauiFirebase.Models;
public class UbicacionVehiculo
{
    [PrimaryKey]
    public int IdUbicacionVehiculo { get; set; }
    [Indexed]
    public int IdVehiculo { get; set; }
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    [JsonIgnore]
    public string? Placa { get; set; } 
    [JsonIgnore]
    public string? NombreConductor { get; set; } 
}
