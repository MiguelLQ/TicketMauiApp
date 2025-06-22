using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiFirebase.Models
{
    public class Premio
    {

        [PrimaryKey, AutoIncrement]
        public int IdPremio { get; set; }

        public string NombrePremio { get; set; } = string.Empty;

        public string DescripcionPremio { get; set; } = string.Empty;

        public int PuntosRequeridos { get; set; }

        public bool EstadoPremio { get; set; }
    }
}
