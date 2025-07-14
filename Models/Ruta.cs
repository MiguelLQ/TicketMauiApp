using SQLite;

namespace MauiFirebase.Models
{
    public class Ruta
    {
        [PrimaryKey]
        public string IdRuta { get; set; } = new Guid().ToString();
        [Indexed]
        public string? IdVehiculo { get; set; }
        public string? DiasDeRecoleccion { get; set; }
        public bool EstadoRuta { get; set; }
        public DateTime FechaRegistroRuta { get; set; }
        public string? PuntosRutaJson { get; set; }
    }
}
