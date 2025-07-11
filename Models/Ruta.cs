using SQLite;

namespace MauiFirebase.Models
{
    public class Ruta
    {
        [PrimaryKey, AutoIncrement]
        public int IdRuta { get; set; }
        [Indexed]
        public int IdVehiculo { get; set; }
        public string? DiasDeRecoleccion { get; set; }
        public bool EstadoRuta { get; set; }
        public DateTime FechaRegistroRuta { get; set; }
        public string? PuntosRutaJson { get; set; }
    }
}
