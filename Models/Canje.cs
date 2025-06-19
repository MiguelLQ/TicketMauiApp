using SQLite;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiFirebase.Models
{
    public class Canje
    {
        [PrimaryKey, AutoIncrement]
        public int IdCanje { get; set; }
        [Indexed]
        public int IdResidente { get; set; }
        [NotNull]
        public int IdPremio { get; set; }
        [NotNull]
        public DateTime FechaCanje { get; set; }
        [NotNull]
        public bool EstadoCanje { get; set; } = true;
    }
}
