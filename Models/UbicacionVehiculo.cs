using SQLite;
using System.Text.Json.Serialization;

namespace MauiFirebase.Models;
public class UbicacionVehiculo
{
    [PrimaryKey]
    public string IdUbicacionVehiculo { get; set; } = Guid.NewGuid().ToString();
    [Indexed]
    public string? IdVehiculo { get; set; }
    public double Latitud { get; set; }
    public double Longitud { get; set; }

    [Ignore]
    public string? Placa { get; set; }

    [Ignore]
    public string? NombreConductor { get; set; }

    public bool Sincronizado { get; set; } = false;
}
