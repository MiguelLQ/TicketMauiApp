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
        public int Id { get; set; }

        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int PuntosRequeridos { get; set; }
    }
}
