using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiFirebase.Models
{
    public class Canje
    {
        [PrimaryKey]
        public string IdCanje { get; set; }= Guid.NewGuid().ToString();
        [Indexed]
        public string? IdResidente { get; set; }
        [NotNull]
        public string? IdPremio { get; set; }
        [NotNull]
        public DateTime FechaCanje { get; set; }
        [NotNull]
        public bool EstadoCanje { get; set; } = true;
        [Ignore] 
        public string? NombreResidente { get; set; }
        [Ignore] 
        public string? ApellidoResidente { get; set; }
        [Ignore] 
        public string? NombrePremio { get; set; }
        [Ignore]
        public string? DescripcionPremio { get; set; }
        public bool Sincronizado { get; set; } = false;

    }
}
